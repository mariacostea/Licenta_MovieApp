using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieByTitleSpec : Specification<Movie>
{
    public MovieByTitleSpec(string title)
    {
        Query.Where(m => m.Title == title);
    }
}