using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Core.DataTransferObjects.Feed;
using MobyLabWebProgramming.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FeedService : IFeedService
{
    private readonly WebAppDatabaseContext _db;

    public FeedService(WebAppDatabaseContext db)
    {
        _db = db;
    }

    public async Task<List<WatchedMovieDTO>> GetWatchedMoviesFeed(Guid userId)
    {
        var friendIds = await _db.Friendships
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToListAsync();
        
        var watched = await _db.UserMovies
            .Where(um => um.IsWatched && friendIds.Contains(um.UserId))
            .Include(um => um.Movie)
            .Include(um => um.User)
            .OrderByDescending(um => um.WatchedOn)
            .Select(um => new WatchedMovieDTO
            {
                MovieTitle = um.Movie.Title,
                PosterUrl = um.Movie.PosterUrl,
                ReleaseYear = um.Movie.Year,
                Description = um.Movie.Description,
                AverageRating = um.Movie.AverageRating,
                FriendName = um.User.Name,
                WatchedAt = um.WatchedOn
            })
            .ToListAsync();

        return watched;
    }

    public async Task<List<ReviewFeedDTO>> GetReviewsFeed(Guid userId)
    {
        var friendIds = await _db.Friendships
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToListAsync();
        
        var reviews = await _db.Reviews
            .Where(r => friendIds.Contains(r.UserId))
            .Include(r => r.User)
            .Include(r => r.Movie)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new ReviewFeedDTO
            {
                MovieTitle = r.Movie.Title,
                PosterUrl = r.Movie.PosterUrl,
                ReleaseYear = r.Movie.Year,
                Description = r.Movie.Description,
                Rating = r.Movie.AverageRating,
                FriendName = r.User.Name,
                ReviewText = r.Content,
                ReviewedAt = r.CreatedAt,
                FriendRating = r.Rating
            })
            .ToListAsync();

        return reviews;
    }

    public async Task<List<EventFeedDTO>> GetEventsFeed(Guid userId)
    {
        var friendIds = await _db.Friendships
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
            .ToListAsync();

        var events = await _db.Events
            .Where(e => friendIds.Contains(e.OrganizerId))
            .Include(e => e.Movie)
            .Include(e => e.Organizer)
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new EventFeedDTO
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Location = e.Location,
                Date = e.Date,
                MaxParticipants = e.MaxParticipants,
                FreeSeats = e.FreeSeats,
                OrganizerId = e.OrganizerId,
                MovieId = e.MovieId,
                CreatedAt = e.CreatedAt,
                MovieTitle = e.Movie.Title,
                MoviePosterUrl = e.Movie.PosterUrl,
                FriendName = e.Organizer.Name
            })
            .ToListAsync();

        return events;
    }
}
