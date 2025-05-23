using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieProjectionByTitleSpec : Specification<Movie, MovieDetailsDTO>
{
    public MovieProjectionByTitleSpec(string keyword)
    {
        var kw = $"%{keyword.Trim().Replace(" ", "%")}%";

        Query.Where(m => EF.Functions.ILike(m.Title, kw));

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