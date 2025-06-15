using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

public class MoviesNotWatchedByUserSpec : Specification<Movie>
{
    public MoviesNotWatchedByUserSpec(Guid userId)
    {
        Query.Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Include(m => m.UserMovies)
            .Where(m => !m.UserMovies.Any(um => um.UserId == userId && um.IsWatched));
    }
}