using MobyLabWebProgramming.Core.DataTransferObjects;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IFeedbackService
{
    Task SubmitFeedbackAsync(FeedbackCreateDTO dto, Guid userId);
    Task<List<FeedbackDTO>> GetAllFeedbacksAsync();
}