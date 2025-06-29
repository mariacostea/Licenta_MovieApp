using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class RecommendedMoviesByUserSpec : Specification<Movie>
{
    public RecommendedMoviesByUserSpec(Guid userId)
    {
        Query
            .Where(m => m.UserMovies.Any(um => um.UserId == userId && um.IsRecommended))
            .Include(m => m.UserMovies);
    }
}