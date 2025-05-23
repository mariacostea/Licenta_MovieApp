using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;
namespace MobyLabWebProgramming.Core.Specifications;

public class WatchedMovieSpec : Specification<UserMovie>
{
    public WatchedMovieSpec(Guid movieId, Guid userId)
    {
        Query.Where(x => x.MovieId == movieId && x.UserId == userId && x.IsWatched);
    }
}
