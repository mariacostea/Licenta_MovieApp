using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Core.Responses;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FriendshipController(IFriendshipService friendshipService) : ControllerBase
{
    [HttpPost("request")]
    public async Task<IActionResult> SendRequest([FromBody] FriendshipRequestCreateDTO dto)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return BadRequest("User not logged in!");

        var friendshipRequest = new FriendshipRequestDTO
        {
            FromUserId = Guid.Parse(userId),
            ToUserId = dto.ToUserId
        };

        var result = await friendshipService.SendRequestAsync(friendshipRequest);
        return this.FromServiceResponse(result);
    }

    [HttpPost("accept/{id:guid}")]
    public async Task<IActionResult> Accept([FromRoute] Guid id)
    {
        var result = await friendshipService.AcceptRequestAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpPost("reject/{id:guid}")]
    public async Task<IActionResult> Reject([FromRoute] Guid id)
    {
        var result = await friendshipService.RejectRequestAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await friendshipService.GetAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("list/{userId:guid}")]
    public async Task<IActionResult> List([FromRoute] Guid userId)
    {
        var result = await friendshipService.ListForUserAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("list/accepted/{userId:guid}")]
    public async Task<IActionResult> AcceptedList([FromRoute] Guid userId)
    {
        var result = await friendshipService.AcceptedListForUserAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("list/pending/{userId:guid}")]
    public async Task<IActionResult> PendingList([FromRoute] Guid userId)
    {
        var result = await friendshipService.PendingListForUserAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("list/received/{userId:guid}")]
    public async Task<IActionResult> ReceivedList([FromRoute] Guid userId)
    {
        var result = await friendshipService.RecievedListForUserAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("users/pending/sent/{userId:guid}")]
    public async Task<IActionResult> GetPendingSentUsers([FromRoute] Guid userId)
    {
        var result = await friendshipService.GetPendingSentUsersAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("users/pending/received/{userId:guid}")]
    public async Task<IActionResult> GetPendingReceivedUsers([FromRoute] Guid userId)
    {
        var result = await friendshipService.GetPendingReceivedUsersAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    [HttpGet("users/friends/{userId:guid}")]
    public async Task<IActionResult> GetFriends([FromRoute] Guid userId)
    {
        var result = await friendshipService.GetFriendsAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }
    
    [HttpPost("unfriend/{friendId:guid}")]
    public async Task<IActionResult> Unfriend([FromRoute] Guid friendId)
    {
        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return BadRequest("User not logged in!");

        var result = await friendshipService.UnfriendAsync(Guid.Parse(userId), friendId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

}
