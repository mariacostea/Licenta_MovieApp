using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MobyLabWebProgramming.Core.Configuration;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public sealed class MovieService : IMovieService
{
    private readonly IRepository<WebAppDatabaseContext> _movieRepo;
    private readonly IOptions<TMDBConfiguration>        _tmdb;

    public MovieService(
        IRepository<WebAppDatabaseContext> movieRepo,
        IOptions<TMDBConfiguration>        tmdbConfig)
    {
        _movieRepo = movieRepo;
        _tmdb      = tmdbConfig;
    }

    /*────────────  CREATE  ────────────*/
    public async Task<Movie> AddOrGetMovieFromApi(MovieDTO dto)
    {
        var existing = await _movieRepo.GetAsync(
            new MovieByTitleAndYearSpec(dto.Title, dto.Year));

        if (existing is not null)
            return existing;

        var tmdb = await FetchMovieFromTmdb(dto.Title, dto.Year ?? 0);
        if (tmdb is null)
            throw new ServerException(HttpStatusCode.BadRequest,
                "Invalid movie: not found in TMDB.");

        var movie = new Movie
        {
            Title       = tmdb.Value.GetProperty("title").GetString()    ?? dto.Title,
            Year        = dto.Year,
            Description = tmdb.Value.GetProperty("overview").GetString() ?? string.Empty
        };

        await _movieRepo.AddAsync(movie);
        return movie;
    }

    /*────────────  READ: paginare globală  ────────────*/
    public async Task<PagedResponse<MovieDetailsDTO>>
        GetAllMoviesAsync(int page, int pageSize)
    {
        var list       = await _movieRepo.ListAsync(
            new MovieProjectionSpec(page: page, pageSize: pageSize));

        var totalCount = await _movieRepo.GetCountAsync(new MovieSpec());
        return new(page, pageSize, totalCount, list);
    }

    /*────────────  READ: titlu + an exact  ────────────*/
    public async Task<MovieDetailsDTO?>
        GetMovieByTitleAsync(string title, int year)
        => await _movieRepo.GetAsync(
            new MovieProjectionByTitleAndYearSpec(title, year));

    /*────────────  READ: gen  ────────────*/
    public async Task<PagedResponse<MovieDetailsDTO>>
        GetMoviesByGenreAsync(string genre, int page, int pageSize)
    {
        var list  = await _movieRepo.ListAsync(
            new MovieProjectionSpec(null, genre, page, pageSize));

        var total = await _movieRepo.GetCountAsync(new MovieSpec(null, genre));
        return new(page, pageSize, total, list);
    }

    /*────────────  READ: an (paginat)  ────────────*/
    public async Task<PagedResponse<MovieDetailsDTO>>
        GetMoviesByYearAsync(int year, int page, int pageSize)
    {
        var list  = await _movieRepo.ListAsync(
            new MovieProjectionSpec(year, null, page, pageSize));

        var total = await _movieRepo.GetCountAsync(new MovieSpec(year));
        return new(page, pageSize, total, list);
    }

    /*────────────  READ: full-text titlu  ────────────*/
    public async Task<IReadOnlyList<MovieDetailsDTO>>
        SearchAllMoviesByTitleAsync(string title)
        => await _movieRepo.ListAsync(new MovieProjectionByTitleSpec(title));

    /*────────────  READ: combinaţie year+genre  ────────────*/
    public async Task<PagedResponse<MovieDetailsDTO>>
        FilterMoviesAsync(int? year, string? genre, int page, int pageSize)
    {
        var list  = await _movieRepo.ListAsync(
            new MovieProjectionSpec(year, genre, page, pageSize));

        var total = await _movieRepo.GetCountAsync(
            new MovieSpec(year, genre));

        return new(page, pageSize, total, list);
    }

    /*── helper TMDB ──*/
    private async Task<JsonElement?> FetchMovieFromTmdb(string title, int year)
    {
        var url =
            $"{_tmdb.Value.BaseUrl}/search/movie?api_key={_tmdb.Value.ApiKey}" +
            $"&query={Uri.EscapeDataString(title)}&year={year}";

        using var http = new HttpClient();
        var resp       = await http.GetAsync(url);
        if (!resp.IsSuccessStatusCode) return null;

        using var doc  = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var results    = doc.RootElement.GetProperty("results");
        return results.GetArrayLength() > 0 ? results[0].Clone() : null;
    }
    
    public async Task<MovieDetailsDTO?> GetMovieByIdAsync(Guid id)
    {
        return await _movieRepo.GetAsync(new MovieProjectionByIdSpec(id));
    }

}
