using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieProjectionByTitleAndYearSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionByTitleAndYearSpec(string title, int year)
    {
        Query.Where(m => m.Title.ToLower() == title.ToLower() && m.Year == year);

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
    }
}