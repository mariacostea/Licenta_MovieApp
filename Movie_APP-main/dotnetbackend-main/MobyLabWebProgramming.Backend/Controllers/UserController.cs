using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;


namespace MobyLabWebProgramming.Backend.Controllers;

/// <summary>
/// This is a controller example for CRUD operations on users.
/// Inject the required services through the constructor.
/// </summary>
[ApiController] // This attribute specifies for the framework to add functionality to the controller such as binding multipart/form-data.
[Route("api/[controller]/[action]")] // The Route attribute prefixes the routes/url paths with template provides as a string, the keywords between [] are used to automatically take the controller and method name.
public class UserController : AuthorizedController
{
    private readonly WebAppDatabaseContext _db;

    public UserController(IUserService userService, WebAppDatabaseContext db) : base(userService)
    {
        _db = db;
    }                                                                                     // Also, you may pass constructor parameters to a base class constructor and call as specific constructor from the base class.
    /// <summary>
    /// This method implements the Read operation (R from CRUD) on a user. 
    /// </summary>
    [Authorize] // You need to use this attribute to protect the route access, it will return a Forbidden status code if the JWT is not present or invalid, and also it will decode the JWT token.
    [HttpGet("{id:guid}")] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetById/<some_guid>.
    public async Task<ActionResult<RequestResponse<UserDTO>>> GetById([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ? 
            FromServiceResponse(await UserService.GetUser(id)) : 
            ErrorMessageResult<UserDTO>(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Read operation (R from CRUD) on page of users.
    /// Generally, if you need to get multiple values from the database use pagination if there are many entries.
    /// It will improve performance and reduce resource consumption for both client and server.
    /// </summary>
    [Authorize]
    [HttpGet] // This attribute will make the controller respond to a HTTP GET request on the route /api/User/GetPage.
    public async Task<ActionResult<RequestResponse<PagedResponse<UserDTO>>>> GetPage([FromQuery] PaginationSearchQueryParams pagination) // The FromQuery attribute will bind the parameters matching the names of
                                                                                                                                         // the PaginationSearchQueryParams properties to the object in the method parameter.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await UserService.GetUsers(pagination)) :
            ErrorMessageResult<PagedResponse<UserDTO>>(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Create operation (C from CRUD) of a user. 
    /// </summary>
    [Authorize]
    [HttpPost] // This attribute will make the controller respond to a HTTP POST request on the route /api/User/Add.
    public async Task<ActionResult<RequestResponse>> Add([FromBody] UserAddDTO user)
    {
        var currentUser = await GetCurrentUser();
        user.Password = PasswordUtils.HashPassword(user.Password);

        return currentUser.Result != null ?
            FromServiceResponse(await UserService.AddUser(user, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Update operation (U from CRUD) on a user. 
    /// </summary>
    [Authorize]
    [HttpPut] // This attribute will make the controller respond to a HTTP PUT request on the route /api/User/Update.
    public async Task<ActionResult<RequestResponse>> Update([FromBody] UserUpdateDTO user) // The FromBody attribute indicates that the parameter is deserialized from the JSON body.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await UserService.UpdateUser(user with
            {
                Password = !string.IsNullOrWhiteSpace(user.Password) ? PasswordUtils.HashPassword(user.Password) : null
            }, currentUser.Result)) :
            ErrorMessageResult(currentUser.Error);
    }

    /// <summary>
    /// This method implements the Delete operation (D from CRUD) on a user.
    /// Note that in the HTTP RFC you cannot have a body for DELETE operations.
    /// </summary>
    [Authorize]
    [HttpDelete("{id:guid}")] // This attribute will make the controller respond to an HTTP DELETE request on the route /api/User/Delete/<some_guid>.
    public async Task<ActionResult<RequestResponse>> Delete([FromRoute] Guid id) // The FromRoute attribute will bind the id from the route to this parameter.
    {
        var currentUser = await GetCurrentUser();

        return currentUser.Result != null ?
            FromServiceResponse(await UserService.DeleteUser(id)) :
            ErrorMessageResult(currentUser.Error);
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
                ErrorCodes.CannotUpdate)));
        }

        var result = await UserService.UpgradeUserToPremium(currentUser.Result!.Id);
        return result.ErrorMessage != null ? ErrorMessageResult(result.ErrorMessage) : Ok(result);
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
            .Select(u => new {
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

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativeUrl = $"/uploads/{fileName}";
        
        var user = await _db.Users.FindAsync(currentUser.Result.Id);
        if (user == null)
            return NotFound("User not found in database.");

        user.ProfilePictureUrl = relativeUrl;
        await _db.SaveChangesAsync();

        return Ok(new { url = relativeUrl });
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


