using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;     
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieSpec : Specification<Movie>
{
    public MovieSpec(int? year = null, string? genre = null, string? keyword = null)
    {
        if (year is not null)
            Query.Where(m => m.Year == year);

        if (!string.IsNullOrWhiteSpace(genre))
            Query.Where(m => m.MovieGenres.Any(x =>             // x = MovieGenre
                x.Genre.Name.ToLower() == genre.ToLower()));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var kw = $"%{keyword.Replace(" ", "%")}%";
            Query.Where(m =>
                EF.Functions.ILike(m.Title, kw) ||
                EF.Functions.ILike(m.Description ?? "", kw));
        }
    }
}