using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

public class ReviewSpec : Specification<Review>
{
    public ReviewSpec(Guid movieId)
    {
        Query.Where(r => r.MovieId == movieId);
    }
}
