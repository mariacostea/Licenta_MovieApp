using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
///  Proiecţia în <see cref="MovieDetailsDTO"/> + paginare.
///  Poate filtra simultan după an şi/sau gen.
/// </summary>
public sealed class MovieProjectionSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionSpec(
        int? year = null,
        string? genre = null,
        int page = 1,
        int pageSize = 40)
    {
        if (year.HasValue)
        {
            Query.Where(m => m.Year == year.Value);
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            Query.Where(m => m.MovieGenres.Any(g =>
                g.Genre.Name.ToLower() == genre.ToLower()));
        }
        
        Query.Select(m => new MovieDetailsDTO
        {
            Id = m.Id,
            Title = m.Title,
            Year = m.Year,
            Description = m.Description,
            AverageRating = m.AverageRating,
            PosterUrl = m.PosterUrl,
            Genres = m.MovieGenres.Select(g => g.Genre.Name).ToList()
        });
        
        Query.Skip((page - 1) * pageSize)
            .Take(pageSize);
    }
}