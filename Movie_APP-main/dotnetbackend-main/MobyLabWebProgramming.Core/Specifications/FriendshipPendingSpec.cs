using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FriendshipPendingSpec : Specification<Core.Entities.Friendship, FriendshipDTO>
{
    public FriendshipPendingSpec(Guid userId)
    {
        Query
            .Where(f => f.RequesterId == userId && f.Status == FriendshipStatus.Pending);
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
                AcceptedAt    = f.AcceptedAt,
                PhotoUrlAddressee = f.Addressee!.ProfilePictureUrl,
                PhotoUrlRequester = f.Requester!.ProfilePictureUrl
            });
    }
}