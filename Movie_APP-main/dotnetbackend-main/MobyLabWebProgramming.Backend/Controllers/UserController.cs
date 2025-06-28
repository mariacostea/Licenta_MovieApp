using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserController : AuthorizedController
{
    private readonly WebAppDatabaseContext _db;
    private readonly ICloudinaryService _cloudinaryService;

    public UserController(
        IUserService userService,
        WebAppDatabaseContext db,
        ICloudinaryService cloudinaryService
    ) : base(userService)
    {
        _db = db;
        _cloudinaryService = cloudinaryService;
    }

    [Authorize]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RequestResponse<UserDTO>>> GetById([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.GetUser(id))
            : ErrorMessageResult<UserDTO>(currentUser.Error);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<PagedResponse<UserDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.GetUsers(pagination))
            : ErrorMessageResult<PagedResponse<UserDTO>>(currentUser.Error);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<RequestResponse>> Add([FromBody] UserAddDTO user)
    {
        var currentUser = await GetCurrentUser();
        user.Password = PasswordUtils.HashPassword(user.Password);

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.AddUser(user, currentUser.Result))
            : ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpPut]
    public async Task<ActionResult<RequestResponse>> Update([FromBody] UserUpdateDTO user)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.UpdateUser(user with
            {
                Password = !string.IsNullOrWhiteSpace(user.Password)
                    ? PasswordUtils.HashPassword(user.Password)
                    : null
            }, currentUser.Result))
            : ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id)
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.DeleteUser(id))
            : ErrorMessageResult(currentUser.Error);
    }

    [Authorize]
    [HttpPost("upgrade-to-premium")]
    public async Task<ActionResult<RequestResponse>> UpgradeToPremium([FromQuery] string code)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null)
            return ErrorMessageResult(currentUser.Error);

        if (code != "PREMIUM2025")
        {
            return BadRequest(RequestResponse.FromError(new ErrorMessage(
                HttpStatusCode.BadRequest,
                "Codul introdus este invalid!",
                ErrorCodes.CannotUpdate
            )));
        }

        var result = await UserService.UpgradeUserToPremium(currentUser.Result!.Id);
        return result.ErrorMessage != null
            ? ErrorMessageResult(result.ErrorMessage)
            : Ok(result);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<RequestResponse<List<UserDTO>>>> GetAll()
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null
            ? FromServiceResponse(await UserService.GetAllUsers(currentUser.Result.Id))
            : ErrorMessageResult<List<UserDTO>>(currentUser.Error);
    }

    [HttpGet("available/{currentUserId:guid}")]
    [Authorize]
    public async Task<ActionResult<RequestResponse<List<UserDTO>>>> GetAvailableUsers([FromRoute] Guid currentUserId)
    {
        var result = await UserService.GetAvailableUsers(currentUserId, HttpContext.RequestAborted);

        return result.Error != null
            ? ErrorMessageResult<List<UserDTO>>(result.Error)
            : FromServiceResponse(result);
    }

    [Authorize]
    [HttpGet("friends/{userId}")]
    public async Task<IActionResult> GetFriends([FromRoute] Guid userId)
    {
        var currentUser = await GetCurrentUser();

        if (currentUser.Result == null || currentUser.Result.Id != userId)
        {
            return Forbid("Nu ai voie să accesezi lista altui utilizator.");
        }

        var friendships = await _db.Friendships
            .Include(f => f.Requester)
            .Include(f => f.Addressee)
            .Where(f => f.Status == FriendshipStatus.Accepted &&
                        (f.RequesterId == userId || f.AddresseeId == userId))
            .ToListAsync();

        var friends = friendships
            .Select(f => f.RequesterId == userId ? f.Addressee : f.Requester)
            .Select(u => new
            {
                u.Id,
                u.Name,
                u.Email
            });

        return Ok(friends);
    }

    [HttpPost("upload-profile-picture")]
    [Authorize]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        var currentUser = await GetCurrentUser();
        if (currentUser.Result == null)
            return Unauthorized("User not found.");

        var cloudUrl = await _cloudinaryService.UploadImageAsync(file);
        if (cloudUrl == null)
            return StatusCode(500, "Image upload failed.");

        var user = await _db.Users.FindAsync(currentUser.Result.Id);
        if (user == null)
            return NotFound("User not found in database.");

        user.ProfilePictureUrl = cloudUrl;
        await _db.SaveChangesAsync();

        return Ok(new { url = cloudUrl });
    }

    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> Count(Guid userId)
    {
        var watchedCount = await _db.UserMovies
            .CountAsync(um => um.UserId == userId && um.IsWatched);

        var recommendedCount = await _db.UserMovies
            .CountAsync(um => um.UserId == userId && um.IsRecommended);

        return Ok(new
        {
            watched = watchedCount,
            recommended = recommendedCount
        });
    }
}
