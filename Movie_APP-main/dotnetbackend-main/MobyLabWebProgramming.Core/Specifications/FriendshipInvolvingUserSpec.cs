using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;


public sealed class FriendshipInvolvingUserSpec : Specification<Friendship>
{
    public FriendshipInvolvingUserSpec(Guid userId)
    {
        Query.Where(f => f.RequesterId == userId || f.AddresseeId == userId);
    }
}
