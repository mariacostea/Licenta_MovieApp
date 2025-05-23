using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Core.Specifications;
using System.Net;


namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FriendshipService : IFriendshipService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public FriendshipService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    /* -------------------- GET -------------------- */
    public async Task<ServiceResponse<FriendshipDTO>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new FriendshipProjectionSpec(id);
        var result = await _repo.GetAsync(spec, ct);

        return result is null
            ? ServiceResponse.FromError<FriendshipDTO>(CommonErrors.EntityNotFound)
            : ServiceResponse.ForSuccess(result);

    }

    /* -------------------- SEND REQUEST -------------------- */
    public async Task<ServiceResponse> SendRequestAsync(FriendshipRequestDTO dto, CancellationToken ct = default)
    {
        if (dto.FromUserId == dto.ToUserId)
            return ServiceResponse.FromError(new ErrorMessage(HttpStatusCode.BadRequest, "Friendship already exists!"));

        var exists = await _repo.AnyAsync(new FriendshipExistsSpec(dto.FromUserId, dto.ToUserId), ct);
        if (exists)
            return ServiceResponse.FromError(new ErrorMessage(HttpStatusCode.BadRequest, "Friendship already exists!"));


        var entity = new Friendship
        {
            RequesterId = dto.FromUserId,
            AddresseeId = dto.ToUserId,
            Status = FriendshipStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(entity, ct);
        return ServiceResponse.ForSuccess();
    }

    /* -------------------- ACCEPT REQUEST -------------------- */
    public async Task<ServiceResponse> AcceptRequestAsync(Guid id, CancellationToken ct = default)
    {
        var friendship = await _repo.GetByIdAsync<Friendship>(id, ct);
        if (friendship is null)
            return ServiceResponse.FromError(CommonErrors.EntityNotFound);

        friendship.Status = FriendshipStatus.Accepted;
        friendship.AcceptedAt = DateTime.UtcNow;

        await _repo.UpdateAsync(friendship, ct);
        return ServiceResponse.ForSuccess();
    }

    /* -------------------- REJECT REQUEST -------------------- */
    public async Task<ServiceResponse> RejectRequestAsync(Guid id, CancellationToken ct = default)
    {
        var friendship = await _repo.GetByIdAsync<Friendship>(id, ct);
        if (friendship is null)
            return ServiceResponse.FromError(CommonErrors.EntityNotFound);

        friendship.Status = FriendshipStatus.Rejected;

        await _repo.UpdateAsync(friendship, ct);
        return ServiceResponse.ForSuccess();
    }

    /* -------------------- LIST FOR USER -------------------- */
    public async Task<ServiceResponse<List<FriendshipDTO>>> ListForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipListForUserSpec(userId);
        var list = await _repo.ListAsync(spec, ct);

        return ServiceResponse<List<FriendshipDTO>>.ForSuccess(list);
    }
}
