using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

public sealed class MovieProjectionByIdSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionByIdSpec(Guid id)
    {
        Query.Where(m => m.Id == id);

        Query.Select(m => new MovieDetailsDTO
        {
            Id            = m.Id,
            Title         = m.Title,
            Year          = m.Year,
            Description   = m.Description,
            PosterUrl     = m.PosterUrl,
            AverageRating = m.AverageRating,
            Genres        = m.MovieGenres.Select(g => g.Genre.Name).ToList()
        });
    }
}