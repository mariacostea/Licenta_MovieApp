using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Workers;

/// <summary>
///  Worker-ul descarcă periodic filmele populare de pe TMDB, adăugând filmele,
///  genurile lipsă şi distribuţia (actori + echipa tehnică de bază).
/// </summary>
public sealed class TmdbImportWorker : BackgroundService
{
    private const int  PagesPerCycle = 0;                
    private static readonly TimeSpan RunInterval = TimeSpan.FromMinutes(1);

    private readonly ILogger<TmdbImportWorker> _logger;
    private readonly IServiceScopeFactory      _scopeFactory;
    private readonly TMDBConfiguration         _tmdb;
    private readonly HttpClient                _client = new(); 

    private int _currentPage = 500; 

    public TmdbImportWorker(
        ILogger<TmdbImportWorker> logger,
        IServiceScopeFactory      scopeFactory,
        IOptions<TMDBConfiguration> tmdbCfg)
    {
        _logger      = logger;
        _scopeFactory = scopeFactory;
        _tmdb         = tmdbCfg.Value;
    }

    // ------------------------------------------------------------------
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
            await Task.Delay(RunInterval, stoppingToken);
        }
    }

    // ------------------------------------------------------------------
    private async Task DoWorkAsync(CancellationToken ct)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepository<WebAppDatabaseContext>>();

        for (var i = 0; i < PagesPerCycle && !ct.IsCancellationRequested; i++)
        {
            var page = _currentPage + i;
            var url  = $"{_tmdb.BaseUrl}/movie/popular?api_key={_tmdb.ApiKey}&page={page}";

            _logger.LogInformation("[TMDB] Fetching page {Page}", page);

            var response = await _client.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("[TMDB] Returned {Status} for page {Page}", response.StatusCode, page);
                continue;
            }

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(ct));
            var added = await ImportPageAsync(doc, repo, ct);

            _logger.LogInformation("[TMDB] Added {Count} movies from page {Page}", added, page);
        }

        _currentPage += PagesPerCycle;
    }

    // ------------------------------------------------------------------
    private async Task<int> ImportPageAsync(JsonDocument pageDoc,
                                            IRepository<WebAppDatabaseContext> repo,
                                            CancellationToken ct)
    {
        int added = 0;

        foreach (var movieElt in pageDoc.RootElement.GetProperty("results").EnumerateArray())
        {
            var title       = movieElt.GetProperty("title").GetString() ?? string.Empty;
            var releaseText = movieElt.GetProperty("release_date").GetString() ?? string.Empty;

            // protecţie contra datelor lipsă → sari peste filmul fără an valid
            if (releaseText.Length < 4 || !int.TryParse(releaseText[..4], out var year))
                continue;

            if (await repo.GetAsync(new MovieByTitleAndYearSpec(title, year), ct) is not null)
                continue;

            var movie = new Movie
            {
                Title       = title,
                Year        = year,
                Description = movieElt.GetProperty("overview").GetString(),
                PosterUrl   = movieElt.TryGetProperty("poster_path", out var poster) &&
                              !string.Equals(poster.GetString(), "null", StringComparison.OrdinalIgnoreCase)
                                ? $"https://image.tmdb.org/t/p/w500{poster.GetString()}"
                                : null
            };
            await repo.AddAsync(movie, ct);
            added++;

            // ---- Genres ----
            foreach (var gid in movieElt.GetProperty("genre_ids").EnumerateArray().Select(e => e.GetInt32()))
            {
                var name = GenreIdToName(gid);
                if (name is null) continue;

                var genre = await repo.GetAsync(new GenreSpec(name), ct) ??
                            await repo.AddAsync(new Genre { Name = name }, ct);

                await repo.AddAsync(new MovieGenre { MovieId = movie.Id, GenreId = genre.Id }, ct);
            }

            // ---- Cast & Crew ----
            var tmdbId = movieElt.GetProperty("id").GetInt32();
            await ImportCreditsAsync(movie, tmdbId, repo, ct);
        }

        return added;
    }

    // ------------------------------------------------------------------
    private async Task ImportCreditsAsync(Movie movie,
                                          int movieTmdbId,
                                          IRepository<WebAppDatabaseContext> repo,
                                          CancellationToken ct)
    {
        var creditsUrl  = $"{_tmdb.BaseUrl}/movie/{movieTmdbId}/credits?api_key={_tmdb.ApiKey}";
        var creditsResp = await _client.GetAsync(creditsUrl, ct);
        if (!creditsResp.IsSuccessStatusCode) return;

        using var creditsDoc = JsonDocument.Parse(await creditsResp.Content.ReadAsStringAsync(ct));
        var root = creditsDoc.RootElement;

        // ---------- CAST (Actors) ----------
        foreach (var cast in root.GetProperty("cast").EnumerateArray())
        {
            var crewId = await EnsureCrewAsync(cast, repo, ct);
            if (crewId is null) continue;

            if (await repo.GetAsync(new MovieCrewSpec(movie.Id, crewId.Value), ct) is null)
                await repo.AddAsync(new MovieCrew
                {
                    MovieId    = movie.Id,
                    CrewId     = crewId.Value,
                    PersonType = PersonTypeEnum.Actor
                }, ct);
        }

        // ---------- CREW (Director / Writer) ----------
        foreach (var crewElem in root.GetProperty("crew").EnumerateArray())
        {
            var job = crewElem.GetProperty("job").GetString() ?? string.Empty;
            if (job is not ("Director" or "Writer")) continue;

            var crewId = await EnsureCrewAsync(crewElem, repo, ct);
            if (crewId is null) continue;

            var pType = job switch
            {
                "Director" => PersonTypeEnum.Director,
                "Writer"   => PersonTypeEnum.Writer,
                _          => PersonTypeEnum.Other
            };

            if (await repo.GetAsync(new MovieCrewSpec(movie.Id, crewId.Value), ct) is null)
                await repo.AddAsync(new MovieCrew
                {
                    MovieId    = movie.Id,
                    CrewId     = crewId.Value,
                    PersonType = pType
                }, ct);
        }
    }

    // ------------------------------------------------------------------
    private async Task<Guid?> EnsureCrewAsync(JsonElement elem,
                                              IRepository<WebAppDatabaseContext> repo,
                                              CancellationToken ct)
    {
        var tmdbId = elem.GetProperty("id").GetInt32();
        var crew   = await repo.GetAsync(new CrewByTmdbIdSpec(tmdbId), ct);
        if (crew is not null) return crew.Id;

        var name  = elem.GetProperty("name").GetString() ?? string.Empty;
        var parts = name.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var first = parts.Length > 0 ? parts[0] : string.Empty;
        var last  = parts.Length > 1 ? parts[1] : string.Empty;
        var path  = elem.TryGetProperty("profile_path", out var pp) ? pp.GetString() : null;

        crew = new Crew
        {
            TmdbId    = tmdbId,
            FirstName = first,
            LastName  = last,
            ImageUrl  = !string.IsNullOrWhiteSpace(path) ? $"https://image.tmdb.org/t/p/w300{path}" : null
        };

        await repo.AddAsync(crew, ct);
        return crew.Id;
    }

    // ------------------------------------------------------------------
    private static string? GenreIdToName(int id) => id switch
    {
        28      => "Action",
        12      => "Adventure",
        16      => "Animation",
        35      => "Comedy",
        80      => "Crime",
        99      => "Documentary",
        18      => "Drama",
        10751   => "Family",
        14      => "Fantasy",
        36      => "History",
        27      => "Horror",
        10402   => "Music",
        9648    => "Mystery",
        10749   => "Romance",
        878     => "Science Fiction",
        10770   => "TV Movie",
        53      => "Thriller",
        10752   => "War",
        37      => "Western",
        _       => null
    };
}
