using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface IFriendshipService
{
    Task<ServiceResponse<FriendshipDTO>> GetAsync(Guid id, CancellationToken ct = default);
    
    Task<ServiceResponse> SendRequestAsync(FriendshipRequestDTO dto, CancellationToken ct = default);

    Task<ServiceResponse> AcceptRequestAsync(Guid friendshipId, CancellationToken ct = default);

    Task<ServiceResponse> RejectRequestAsync(Guid friendshipId, CancellationToken ct = default);

    Task<ServiceResponse<List<FriendshipDTO>>> ListForUserAsync(Guid userId, CancellationToken ct = default);
}