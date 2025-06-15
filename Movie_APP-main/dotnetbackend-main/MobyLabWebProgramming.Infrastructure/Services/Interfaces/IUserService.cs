using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be used to manage user information.
/// As most routes and business logic will need to know what user is currently using the backend, this service will be the most used.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// GetUser will provide the information about a user given its user Id.
    /// </summary>
    Task<ServiceResponse<UserDTO>> GetUser(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// GetUsers returns a page with user information from the database.
    /// </summary>
    Task<ServiceResponse<PagedResponse<UserDTO>>> GetUsers(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login responds to a user login request with the JWT token and user information.
    /// The implementation of this method should:
    /// 1. Găsească userul după email.
    /// 2. Verifice parola.
    /// 3. Verifice că userul are <c>EmailConfirmed == true</c>.
    /// 4. Genereze tokenul (via LoginService / JWT).
    /// </summary>
    Task<ServiceResponse<LoginResponseDTO>> Login(LoginDTO login, CancellationToken cancellationToken = default);

    /// <summary>
    /// GetUserCount returns the number of users in the database.
    /// </summary>
    Task<ServiceResponse<int>> GetUserCount(CancellationToken cancellationToken = default);

    /// <summary>
    /// AddUser adds a user and verifies if the requesting user has permissions to add one.
    /// If the requesting user is null then no verification is performed as it indicates that the application is calling.
    /// </summary>
    Task<ServiceResponse> AddUser(UserAddDTO user, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// UpdateUser updates a user and verifies if the requesting user has permissions to update it, if the user is his own then that should be allowed.
    /// If the requesting user is null then no verification is performed as it indicates that the application is calling.
    /// </summary>
    Task<ServiceResponse> UpdateUser(UserUpdateDTO user, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// DeleteUser deletes a user and verifies if the requesting user has permissions to delete it, if the user is his own then that should be allowed.
    /// If the requesting user is null then no verification is performed as it indicates that the application is calling.
    /// </summary>
    Task<ServiceResponse> DeleteUser(Guid id, UserDTO? requestingUser = null, CancellationToken cancellationToken = default);
    
    Task<ServiceResponse> ConfirmEmail(string token, CancellationToken cancellationToken = default);

    Task<User?> GetUserByEmail(string email, CancellationToken cancellationToken = default);
    
    Task<RequestResponse> UpgradeUserToPremium(Guid userId);
    Task<ServiceResponse<List<UserDTO>>> GetAllUsers(Guid currentUserId, CancellationToken cancellationToken = default);
    
    Task<ServiceResponse<List<UserDTO>>> GetAvailableUsers(Guid currentUserId, CancellationToken cancellationToken = default);


}
