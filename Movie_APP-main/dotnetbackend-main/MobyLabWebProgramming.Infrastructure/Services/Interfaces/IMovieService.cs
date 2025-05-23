// Infrastructure ▸ Services ▸ Interfaces ▸ IMovieService.cs
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IMovieService
{
    /* CREATE */
    Task<Movie> AddOrGetMovieFromApi(MovieDTO dto);

    /* READ – listări standard */
    Task<PagedResponse<MovieDetailsDTO>> GetAllMoviesAsync   (int page, int pageSize);
    Task<MovieDetailsDTO?>               GetMovieByTitleAsync(string title, int year);
    Task<PagedResponse<MovieDetailsDTO>> GetMoviesByGenreAsync(string genre, int page, int pageSize);
    Task<PagedResponse<MovieDetailsDTO>> GetMoviesByYearAsync (int year,  int page, int pageSize);

    /* READ – căutare full-text în titlu (fără paginare; folosită la validări) */
    Task<IReadOnlyList<MovieDetailsDTO>> SearchAllMoviesByTitleAsync(string title);

    /* READ – filtru combinat year ± genre (ambele opţionale) */
    Task<PagedResponse<MovieDetailsDTO>> FilterMoviesAsync(int? year, string? genre,
        int page, int pageSize);
    
    Task<MovieDetailsDTO?> GetMovieByIdAsync(Guid id);
}