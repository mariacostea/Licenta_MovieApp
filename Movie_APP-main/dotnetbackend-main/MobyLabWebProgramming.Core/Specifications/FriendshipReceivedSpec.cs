using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FriendshipReceivedSpec : Specification<Core.Entities.Friendship, FriendshipDTO>
{
    public FriendshipReceivedSpec(Guid userId)
    {
        Query
            .Where(f => f.AddresseeId == userId && f.Status == FriendshipStatus.Pending);
        Query
            .Select(f => new FriendshipDTO
            {
                Id            = f.Id,
                RequesterId   = f.RequesterId,
                RequesterName = f.Requester!.Name,
                AddresseeId   = f.AddresseeId,
                AddresseeName = f.Addressee!.Name,
                Status        = f.Status.ToString(),
                RequestedAt   = f.RequestedAt,
                AcceptedAt    = f.AcceptedAt
            });
    }
}