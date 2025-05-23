using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class UserEventSpec : Specification<UserEvent>
{
    public UserEventSpec(Guid eventId, Guid userId)
    {
        Query.Where(ue => ue.EventId == eventId && ue.UserId == userId);
    }
}