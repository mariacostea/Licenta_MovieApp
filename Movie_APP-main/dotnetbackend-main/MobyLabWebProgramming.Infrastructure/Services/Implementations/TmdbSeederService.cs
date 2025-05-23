using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class TmdbSeederService : ITmdbSeederService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;
    private readonly string _apiKey;
    private readonly string _baseUrl;

    public TmdbSeederService(IRepository<WebAppDatabaseContext> repo, IOptions<TMDBConfiguration> tmdbOptions)
    {
        _repo = repo;
        _apiKey = tmdbOptions.Value.ApiKey;
        _baseUrl = tmdbOptions.Value.BaseUrl;
    }

    public async Task SeedGenresAndMoviesAsync()
    {
        using var client = new HttpClient();

        // ===================
        // 1. Seed GENRES
        // ===================
        var genreUrl = $"{_baseUrl}/genre/movie/list?api_key={_apiKey}";
        var genreRes = await client.GetAsync(genreUrl);
        genreRes.EnsureSuccessStatusCode();

        var genreJson = await genreRes.Content.ReadAsStringAsync();
        using var genreDoc = JsonDocument.Parse(genreJson);
        var genreArray = genreDoc.RootElement.GetProperty("genres");

        foreach (var genreElem in genreArray.EnumerateArray())
        {
            var tmdbId = genreElem.GetProperty("id").GetInt32();
            var name = genreElem.GetProperty("name").GetString();

            var exists = await _repo.GetAsync(new GenreSpec(tmdbId));
            if (exists is null)
            {
                await _repo.AddAsync(new Genre
                {
                    TmdbId = tmdbId,
                    Name = name!
                });
            }
        }

        // ===================
        // 2. Seed MOVIES
        // ===================
        for (int page = 1; page <= 2; page++)
        {
            var movieUrl = $"{_baseUrl}/movie/popular?api_key={_apiKey}&page={page}";
            var movieRes = await client.GetAsync(movieUrl);
            movieRes.EnsureSuccessStatusCode();

            var movieJson = await movieRes.Content.ReadAsStringAsync();
            using var movieDoc = JsonDocument.Parse(movieJson);
            var movieArray = movieDoc.RootElement.GetProperty("results");

            foreach (var movieElem in movieArray.EnumerateArray())
            {
                var tmdbId = movieElem.GetProperty("id").GetInt32();
                var title = movieElem.GetProperty("title").GetString();
                var description = movieElem.GetProperty("overview").GetString();
                var releaseDate = movieElem.TryGetProperty("release_date", out var dateProp)
                    && DateTime.TryParse(dateProp.GetString(), out var release)
                        ? release
                        : DateTime.MinValue;
                var year = releaseDate != DateTime.MinValue ? releaseDate.Year : (int?)null;
                var posterPath = movieElem.GetProperty("poster_path").GetString();

                var exists = await _repo.GetAsync(new MovieByTitleAndYearSpec(title!, year ?? 0));
                if (exists is not null) continue;

                var movie = new Movie
                {
                    TmdbId = tmdbId,
                    Title = title!,
                    Description = description,
                    Year = year,
                    PosterUrl = posterPath is not null ? $"https://image.tmdb.org/t/p/w500{posterPath}" : null
                };

                await _repo.AddAsync(movie);
                
                if (movieElem.TryGetProperty("genre_ids", out var genreIds))
                {
                    foreach (var genreIdJson in genreIds.EnumerateArray())
                    {
                        var genreId = genreIdJson.GetInt32();
                        var localGenre = await _repo.GetAsync(new GenreSpec(genreId));
                        if (localGenre is not null)
                        {
                            var mg = new MovieGenre
                            {
                                MovieId = movie.Id,
                                GenreId = localGenre.Id
                            };

                            await _repo.AddAsync(mg);
                        }
                    }
                }
            }
        }
    }
}
