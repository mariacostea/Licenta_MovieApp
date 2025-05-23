using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public class ReviewProjectionSpec : Specification<Review, ReviewDTO>
{
    public ReviewProjectionSpec(Guid movieId)
    {
        Query.Where(r => r.MovieId == movieId)
            .Include(r => r.User); 
            
        Query.Select(r => new ReviewDTO
            {
                Id = r.Id,
                MovieId = r.MovieId,
                Content = r.Content,
                Rating = r.Rating,
                Author = r.User.Name,
                UserId = r.UserId
            });
    }
}
