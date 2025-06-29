using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications
{
    public class WatchedMoviesByUserSpec : Specification<Movie>
    {
        public WatchedMoviesByUserSpec(Guid userId)
        {
            Query
                .Where(m => m.UserMovies.Any(um => um.UserId == userId))
                .Include(m => m.UserMovies);
        }
    }
}