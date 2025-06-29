using System.Net;
using MobyLabWebProgramming.Core.Constants;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class UserService(
    IRepository<WebAppDatabaseContext> repository,
    ILoginService loginService,
    IMailService mailService
) : IUserService
{
    public async Task<ServiceResponse<UserDTO>> GetUser(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await repository.GetAsync(new UserProjectionSpec(id), cancellationToken);
        return result != null
            ? ServiceResponse.ForSuccess(result)
            : ServiceResponse.FromError<UserDTO>(CommonErrors.UserNotFound);
    }

    public async Task<ServiceResponse<PagedResponse<UserDTO>>> GetUsers(
        PaginationSearchQueryParams pagination,
        CancellationToken cancellationToken = default
    )
    {
        var result = await repository.PageAsync(pagination, new UserProjectionSpec(pagination.Search), cancellationToken);
        return ServiceResponse.ForSuccess(result);
    }

    public async Task<ServiceResponse<LoginResponseDTO>> Login(LoginDTO login, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetAsync(new UserSpec(login.Email), cancellationToken);

        if (user == null)
        {
            return ServiceResponse.FromError<LoginResponseDTO>(CommonErrors.UserNotFound);
        }

        if (user.Password != login.Password)
        {
            return ServiceResponse.FromError<LoginResponseDTO>(new(
                HttpStatusCode.BadRequest,
                "Wrong password!",
                ErrorCodes.WrongPassword
            ));
        }

        if (!user.EmailConfirmed)
        {
            return ServiceResponse.FromError<LoginResponseDTO>(new(
                HttpStatusCode.BadRequest,
                "Email not confirmed!",
                ErrorCodes.EmailNotConfirmed
            ));
        }

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            ProfilePictureUrl = user.ProfilePictureUrl
        };

        var token = loginService.GetToken(userDTO, DateTime.UtcNow, TimeSpan.FromDays(7));

        var response = new LoginResponseDTO
        {
            User = userDTO,
            Token = token
        };

        return ServiceResponse<LoginResponseDTO>.ForSuccess(response);
    }

    public async Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default)
        => ServiceResponse.ForSuccess(await repository.GetCountAsync<User>(cancellationToken));

    public async Task<ServiceResponse> AddUser(UserAddDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.Forbidden,
                "Only the admin can add users!",
                ErrorCodes.CannotAdd
            ));
        }

        var existing = await repository.GetAsync(new UserSpec(user.Email), cancellationToken);
        if (existing != null)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.Conflict,
                "The user already exists!",
                ErrorCodes.UserAlreadyExists
            ));
        }

        var newUser = new User
        {
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Password = user.Password,
            EmailConfirmed = false,
            EmailConfirmationToken = null
        };

        await repository.AddAsync(newUser, cancellationToken);

        var token = Guid.NewGuid().ToString();
        newUser.EmailConfirmationToken = token;
        await repository.UpdateAsync(newUser, cancellationToken);

        var confirmLink = $"https://licenta-backend-nf1m.onrender.com/api/Authorization/ConfirmEmail?token={token}";
        var body = $@"
            <h2>Welcome, {newUser.Name}!</h2>
            <p>Click the link below to confirm your email:</p>
            <a href='{confirmLink}'>Confirm Email</a>
            <br/><br/>
            <i>This link is valid once.</i>
        ";

        await mailService.SendMail(
            newUser.Email,
            "Confirm your email",
            body,
            true,
            "My App",
            cancellationToken
        );

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateUser(UserUpdateDTO user, UserDTO? requestingUser, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != user.Id)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.Forbidden,
                "Only the admin or the own user can update the user!",
                ErrorCodes.CannotUpdate
            ));
        }

        var entity = await repository.GetAsync(new UserSpec(user.Id), cancellationToken);
        if (entity == null)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.NotFound,
                "User not found!",
                ErrorCodes.EntityNotFound
            ));
        }

        entity.Name = user.Name ?? entity.Name;
        entity.Password = user.Password ?? entity.Password;

        await repository.UpdateAsync(entity, cancellationToken);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default)
    {
        if (requestingUser != null && requestingUser.Role != UserRoleEnum.Admin && requestingUser.Id != id)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.Forbidden,
                "Only the admin or the own user can delete the user!",
                ErrorCodes.CannotDelete
            ));
        }

        await repository.DeleteAsync<User>(id, cancellationToken);
        return ServiceResponse.ForSuccess();
    }
    
    public async Task<ServiceResponse> ConfirmEmail(string token, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetAsync(new UserEmailTokenSpec(token), cancellationToken);

        if (user == null)
        {
            return ServiceResponse.FromError(new(
                HttpStatusCode.BadRequest,
                "Invalid token.",
                ErrorCodes.EntityNotFound
            ));
        }

        user.EmailConfirmed = true;
        user.EmailConfirmationToken = null;

        await repository.UpdateAsync(user, cancellationToken);

        return ServiceResponse.ForSuccess();
    }

    public async Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default)
    {
        return await repository.GetAsync(new UserSpec(email), cancellationToken);
    }

    public async Task<RequestResponse> UpgradeUserToPremium(Guid userId)
    {
        var user = await repository.GetAsync<User>(userId);

        if (user == null)
            return RequestResponse<string>.FromError(
                new ErrorMessage(
                    HttpStatusCode.NotFound,
                    "Utilizatorul nu a fost găsit.",
                    ErrorCodes.EntityNotFound
                )
            );

        user.Role = UserRoleEnum.Premium;

        await repository.UpdateAsync(user);

        return RequestResponse.FromError(null);
    }
    
    public async Task<ServiceResponse<List<UserDTO>>> GetAllUsers(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var users = await repository.ListAsync(new UserProjectionSpec(), cancellationToken);
        
        var filteredUsers = users.Where(u => u.Id != currentUserId).ToList();

        return ServiceResponse<List<UserDTO>>.ForSuccess(filteredUsers);
    }

    public async Task<ServiceResponse<List<UserDTO>>> GetAvailableUsers(Guid currentUserId, CancellationToken cancellationToken = default)
    {
        var allUsers = await repository.ListAsync(new UserProjectionSpec(), cancellationToken);
        var friendships = await repository.ListAsync(new FriendshipInvolvingUserSpec(currentUserId), cancellationToken);

        var relatedUserIds = friendships
            .Select(f => f.RequesterId == currentUserId ? f.AddresseeId : f.RequesterId)
            .ToHashSet();

        var availableUsers = allUsers
            .Where(u => u.Id != currentUserId && !relatedUserIds.Contains(u.Id))
            .ToList();

        return ServiceResponse<List<UserDTO>>.ForSuccess(availableUsers);
    }

    public async Task<ServiceResponse<UserExtendedProfileDTO>> GetExtendedProfile(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await repository.GetAsync(new UserProjectionSpec(userId), cancellationToken);
        if (user == null)
            return ServiceResponse.FromError<UserExtendedProfileDTO>(CommonErrors.UserNotFound);

        var watched = await repository.ListAsync(new WatchedMoviesByUserSpec(userId), cancellationToken);
        var recommended = await repository.ListAsync(new RecommendedMoviesByUserSpec(userId), cancellationToken);

        // Folosim EventDTO direct
        var events = await repository.ListAsync(new EventProjectionSpec(userId, "Organizer"), cancellationToken);

        var dto = new UserExtendedProfileDTO
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            ProfilePictureUrl = user.ProfilePictureUrl,

            WatchedCount = watched.Count,
            RecommendedCount = recommended.Count,

            WatchedMovies = watched.Select(m => new MovieSimpleDTO
            {
                Id = m.Id,
                Title = m.Title,
                PosterUrl = m.PosterUrl
            }).ToList(),

            RecommendedMovies = recommended.Select(m => new MovieSimpleDTO
            {
                Id = m.Id,
                Title = m.Title,
                PosterUrl = m.PosterUrl
            }).ToList(),
            
            OrganizedEvents = events
        };

        return ServiceResponse.ForSuccess(dto);
    }


    
}
