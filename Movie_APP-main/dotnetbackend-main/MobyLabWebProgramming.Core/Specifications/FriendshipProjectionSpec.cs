using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FriendshipProjectionSpec : Specification<Core.Entities.Friendship, FriendshipDTO>
{
    public FriendshipProjectionSpec(Guid id)
    {
        Query
            .Where(f => f.Id == id);
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