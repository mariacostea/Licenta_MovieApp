using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

public sealed class ReviewByUserAndMovieSpec : Specification<Review>
{
    public ReviewByUserAndMovieSpec(Guid userId, Guid movieId)
    {
        Query.Where(r => r.UserId == userId && r.MovieId == movieId);
    }
}