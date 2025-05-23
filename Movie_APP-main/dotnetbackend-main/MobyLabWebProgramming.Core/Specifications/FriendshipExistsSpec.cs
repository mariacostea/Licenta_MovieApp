using Ardalis.Specification;
namespace MobyLabWebProgramming.Core.Specifications;

public sealed class FriendshipExistsSpec : Specification<Core.Entities.Friendship>
{
    public FriendshipExistsSpec(Guid u1, Guid u2)
    {
        Query.Where(f =>
            (f.RequesterId == u1 && f.AddresseeId == u2) ||
            (f.RequesterId == u2 && f.AddresseeId == u1));
    }
}