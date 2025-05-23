using MobyLabWebProgramming.Core.DataTransferObjects;
namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IReviewService
{
    Task<List<ReviewDTO>> GetReviewsByMovieTitleAndYearAsync(string title, int year);
    Task AddReviewAsync(ReviewDTO dto, Guid userId);
    
    Task UpdateReviewAsync(Guid reviewId, ReviewDTO dto, Guid userId);
    
    Task DeleteReviewAsync(Guid reviewId, Guid userId);
    
}
