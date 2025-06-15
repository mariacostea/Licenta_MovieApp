using System;
using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;
public class MoviesWatchedByUserSpec : Specification<Movie>
{
    public MoviesWatchedByUserSpec(Guid userId)
    {
        Query
            .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
            .Include(m => m.Reviews)
            .Include(m => m.UserMovies)
            .Include(m => m.MovieCrews).ThenInclude(mc => mc.Crew)
            .Where(m => m.UserMovies.Any(um => um.UserId == userId && um.IsWatched));
        
    }
}
