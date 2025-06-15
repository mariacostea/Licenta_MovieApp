using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController : ControllerBase
{
    private readonly IFeedService _feedService;

    public FeedController(IFeedService feedService)
    {
        _feedService = feedService;
    }

    [HttpGet("watched/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetWatched(Guid userId)
    {
        var result = await _feedService.GetWatchedMoviesFeed(userId);
        return Ok(result);
    }

    [HttpGet("reviews/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetReviews(Guid userId)
    {
        var result = await _feedService.GetReviewsFeed(userId);
        return Ok(result);
    }

    [HttpGet("events/{userId}")]
    [Authorize]
    public async Task<IActionResult> GetEvents(Guid userId)
    {
        var result = await _feedService.GetEventsFeed(userId);
        return Ok(result);
    }
}