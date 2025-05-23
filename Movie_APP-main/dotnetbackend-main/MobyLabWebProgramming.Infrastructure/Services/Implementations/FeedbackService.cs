using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public FeedbackService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    public async Task SubmitFeedbackAsync(FeedbackCreateDTO dto, Guid userId)
    {
        var feedback = new Feedback
        {
            FeedbackType = dto.FeedbackType,
            Rating = dto.Rating,
            AgreeToTerms = dto.AgreeToTerms,
            Comment = dto.Comment,
            UserId = userId
        };

        await _repo.AddAsync(feedback);
    }

    public async Task<List<FeedbackDTO>> GetAllFeedbacksAsync()
    {
        var list = await _repo.ListAsync(new FeedbackAllSpec());

        return list.Select(f => new FeedbackDTO
        {
            Id = f.Id,
            FeedbackType = f.FeedbackType,
            Rating = f.Rating,
            AgreeToTerms = f.AgreeToTerms,
            Comment = f.Comment,
            UserId = f.UserId
        }).ToList();
    }
}