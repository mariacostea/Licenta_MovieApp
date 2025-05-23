using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieByTitleAndYearSpec : Specification<Movie>
{
    public MovieByTitleAndYearSpec(string title, int? year)
    {
        Query.Where(m => m.Title == title && m.Year == year);
    }
}