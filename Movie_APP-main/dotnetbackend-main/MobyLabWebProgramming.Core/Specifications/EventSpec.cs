using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class EventSpec : Specification<Event>
{
    public EventSpec(Guid id, bool includeDetails = false)
    {
        Query.Where(e => e.Id == id);

        if (includeDetails)
        {
            Query.Include(e => e.Organizer)
                .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.User);
        }
    }
    
}