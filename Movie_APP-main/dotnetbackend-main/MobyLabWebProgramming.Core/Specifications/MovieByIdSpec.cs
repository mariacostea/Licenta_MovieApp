using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieByIdSpec : Specification<Movie>
{
    public MovieByIdSpec(Guid movieId)
    {
        Query.Where(m => m.Id == movieId);
    }
}
