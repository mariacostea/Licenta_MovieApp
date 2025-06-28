using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Handlers;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Core.Enums;
using System.Net;
using MobyLabWebProgramming.Core.Errors;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthorizationController(IUserService userService, ILoginService loginService) : BaseResponseController
{
    private readonly ILoginService _loginService = loginService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse>> Register([FromBody] RegisterDTO register)
    {
        var hashedUser = new UserAddDTO
        {
            Name = register.Username,
            Email = register.Email,
            Password = PasswordUtils.HashPassword(register.Password),
            Role = UserRoleEnum.User,
            ProfilePictureUrl = null
        };

        return FromServiceResponse(await userService.AddUser(hashedUser));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse>> ConfirmEmail([FromQuery] string token)
    {
        return FromServiceResponse(await userService.ConfirmEmail(token));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse<LoginResponseDTO>>> Login([FromBody] LoginDTO login)
    {
        var user = await userService.GetUserByEmail(login.Email);

        if (user == null)
        {
            return ErrorMessageResult<LoginResponseDTO>(new ErrorMessage(
                HttpStatusCode.NotFound,
                "Emailul introdus nu există.",
                ErrorCodes.UserAlreadyExists
            ));
        }

        if (!PasswordUtils.VerifyPassword(login.Password, user.Password))
        {
            return ErrorMessageResult<LoginResponseDTO>(new ErrorMessage(
                HttpStatusCode.Unauthorized,
                "Parola introdusă este greșită.",
                ErrorCodes.WrongPassword
            ));
        }

        if (!user.EmailConfirmed)
        {
            return ErrorMessageResult<LoginResponseDTO>(new ErrorMessage(
                HttpStatusCode.Unauthorized,
                "Confirmă adresa de email.",
                ErrorCodes.EmailNotConfirmed
            ));
        }

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        };

        var token = _loginService.GetToken(userDTO, DateTime.UtcNow, TimeSpan.FromDays(7));

        var response = new LoginResponseDTO
        {
            User = userDTO,
            Token = token
        };

        return Ok(RequestResponse<LoginResponseDTO>.Success(response));
    }
}
