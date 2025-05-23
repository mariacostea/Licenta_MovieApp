using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;
public sealed class MovieByYearProjectionSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieByYearProjectionSpec(int year, int page = 1, int pageSize = int.MaxValue)
    {
        Query.Where(m => m.Year == year)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        Query.Select(m => new MovieDetailsDTO
        {
            Id            = m.Id,
            Title         = m.Title,
            Year          = m.Year,
            Description   = m.Description,
            AverageRating = m.AverageRating,
            PosterUrl     = m.PosterUrl,
            Genres        = m.MovieGenres.Select(g => g.Genre.Name).ToList()
        });
    }
}