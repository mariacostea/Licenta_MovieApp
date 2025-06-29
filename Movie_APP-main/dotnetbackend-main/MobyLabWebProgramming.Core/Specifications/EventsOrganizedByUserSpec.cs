using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class EventsOrganizedByUserSpec : Specification<Event>
{
    public EventsOrganizedByUserSpec(Guid userId)
    {
        Query
            .Where(e => e.OrganizerId == userId);
    }
}