using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class UserEventIsOrganizerSpec : Specification<UserEvent>
{
    public UserEventIsOrganizerSpec(Guid eventId, Guid userId)
    {
        Query.Where(ue => ue.EventId == eventId && ue.UserId == userId && ue.Status == "Organizer");
    }
}