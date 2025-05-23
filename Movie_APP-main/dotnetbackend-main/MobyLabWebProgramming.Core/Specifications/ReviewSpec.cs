using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

public class ReviewSpec : Specification<Review>
{
    public ReviewSpec(Guid movieId)
    {
        Query.Where(r => r.MovieId == movieId);
    }
    public ReviewSpec(Guid movieId, Guid userId)
    {
        Query.Where(r => r.MovieId == movieId && r.UserId == userId);
    }
}
