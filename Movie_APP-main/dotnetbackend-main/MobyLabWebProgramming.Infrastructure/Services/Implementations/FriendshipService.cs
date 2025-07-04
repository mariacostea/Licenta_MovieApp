﻿using System.Net;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class FriendshipService : IFriendshipService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public FriendshipService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    public async Task<ServiceResponse<FriendshipDTO>> GetAsync(Guid id, CancellationToken ct = default)
    {
        var spec = new FriendshipProjectionSpec(id);
        var result = await _repo.GetAsync(spec, ct);

        return result is null
            ? ServiceResponse.FromError<FriendshipDTO>(CommonErrors.EntityNotFound)
            : ServiceResponse.ForSuccess(result);
    }

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
            RequestedAt = DateTime.UtcNow,
        };

        await _repo.AddAsync(entity, ct);
        return ServiceResponse.ForSuccess();
    }

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

    public async Task<ServiceResponse> RejectRequestAsync(Guid id, CancellationToken ct = default)
    {
        var friendship = await _repo.GetByIdAsync<Friendship>(id, ct);
        if (friendship is null)
            return ServiceResponse.FromError(CommonErrors.EntityNotFound);

        friendship.Status = FriendshipStatus.Rejected;

        await _repo.UpdateAsync(friendship, ct);
        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse<List<FriendshipDTO>>> ListForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipListForUserSpec(userId);
        var list = await _repo.ListAsync(spec, ct);

        return ServiceResponse<List<FriendshipDTO>>.ForSuccess(list);
    }

    public async Task<ServiceResponse<List<FriendshipDTO>>> AcceptedListForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipAcceptedSpec(userId);
        var list = await _repo.ListAsync(spec, ct);

        return ServiceResponse<List<FriendshipDTO>>.ForSuccess(list);
    }

    public async Task<ServiceResponse<List<FriendshipDTO>>> PendingListForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipPendingSpec(userId);
        var list = await _repo.ListAsync(spec, ct);

        return ServiceResponse<List<FriendshipDTO>>.ForSuccess(list);
    }

    public async Task<ServiceResponse<List<FriendshipDTO>>> RecievedListForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipReceivedSpec(userId);
        var list = await _repo.ListAsync(spec, ct);

        return ServiceResponse<List<FriendshipDTO>>.ForSuccess(list);
    }

    public async Task<ServiceResponse<List<UserDTO>>> GetFriendsAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipAcceptedSpec(userId);
        var friendships = await _repo.ListAsync(spec, ct);

        var userIds = friendships.Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId).ToList();
        var users = await _repo.ListAsync(new UserProjectionSpec(), ct);
        var result = users.Where(u => userIds.Contains(u.Id)).ToList();

        return ServiceResponse<List<UserDTO>>.ForSuccess(result);
    }

    public async Task<ServiceResponse<List<UserDTO>>> GetPendingSentUsersAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipPendingSpec(userId);
        var friendships = await _repo.ListAsync(spec, ct);

        var userIds = friendships.Select(f => f.AddresseeId).ToList();
        var users = await _repo.ListAsync(new UserProjectionSpec(), ct);
        var result = users.Where(u => userIds.Contains(u.Id)).ToList();

        return ServiceResponse<List<UserDTO>>.ForSuccess(result);
    }

    public async Task<ServiceResponse<List<UserDTO>>> GetPendingReceivedUsersAsync(Guid userId, CancellationToken ct = default)
    {
        var spec = new FriendshipReceivedSpec(userId);
        var friendships = await _repo.ListAsync(spec, ct);

        var userIds = friendships.Select(f => f.RequesterId).ToList();
        var users = await _repo.ListAsync(new UserProjectionSpec(), ct);
        var result = users.Where(u => userIds.Contains(u.Id)).ToList();

        return ServiceResponse<List<UserDTO>>.ForSuccess(result);
    }
    
    public async Task<ServiceResponse> UnfriendAsync(Guid userId, Guid friendId, CancellationToken ct)
    {
        var friendship = await _repo.GetAsQueryable<Friendship>()
            .Where(f =>
                (f.RequesterId == userId && f.AddresseeId == friendId ||
                 f.RequesterId == friendId && f.AddresseeId == userId) &&
                f.Status == FriendshipStatus.Accepted)
            .FirstOrDefaultAsync(ct);

        if (friendship is null)
        {
            return ServiceResponse.FromError(new ErrorMessage(
                System.Net.HttpStatusCode.NotFound,
                "Friendship does not exist or is not accepted.",
                ErrorCodes.EntityNotFound
            ));
        }

        await _repo.DeleteAsync<Friendship>(friendship.Id, ct);

        return ServiceResponse.ForSuccess("Friendship removed.");
    }


}
