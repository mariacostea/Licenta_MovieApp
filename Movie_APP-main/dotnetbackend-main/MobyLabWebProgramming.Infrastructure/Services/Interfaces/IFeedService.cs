using MobyLabWebProgramming.Core.DataTransferObjects.Feed;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IFeedService
{
    Task<List<WatchedMovieDTO>> GetWatchedMoviesFeed(Guid userId);
    Task<List<ReviewFeedDTO>> GetReviewsFeed(Guid userId);
    Task<List<EventFeedDTO>> GetEventsFeed(Guid userId);
}