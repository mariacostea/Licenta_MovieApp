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
public class FriendshipController(IFriendshipService service) : ControllerBase
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

        var result = await service.SendRequestAsync(friendshipRequest);
        return this.FromServiceResponse(result);
    }
    
    // --- Accept a friend request ---
    [HttpPost("accept/{id:guid}")]
    public async Task<IActionResult> Accept([FromRoute] Guid id)
    {
        var result = await service.AcceptRequestAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }

    // --- Reject a friend request ---
    [HttpPost("reject/{id:guid}")]
    public async Task<IActionResult> Reject([FromRoute] Guid id)
    {
        var result = await service.RejectRequestAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }


    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var result = await service.GetAsync(id, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }
    
    [HttpGet("list/{userId:guid}")]
    public async Task<IActionResult> List([FromRoute] Guid userId)
    {
        var result = await service.ListForUserAsync(userId, HttpContext.RequestAborted);
        return this.FromServiceResponse(result);
    }
}
