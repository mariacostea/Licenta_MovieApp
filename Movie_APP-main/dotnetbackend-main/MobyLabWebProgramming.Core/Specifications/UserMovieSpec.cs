using MobyLabWebProgramming.Core.Entities;
using Ardalis.Specification;

namespace MobyLabWebProgramming.Core.Specifications;

public class UserMovieSpec : Specification<UserMovie>
{
    public UserMovieSpec(Guid userId, Guid movieId)
    {
        Query.Where(um => um.UserId == userId && um.MovieId == movieId && um.IsWatched == true);
    }
}
