using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieByYearSpec : Specification<Movie>
{
    public MovieByYearSpec(int year)
    {
        Query.Where(m => m.Year == year);
    }
}