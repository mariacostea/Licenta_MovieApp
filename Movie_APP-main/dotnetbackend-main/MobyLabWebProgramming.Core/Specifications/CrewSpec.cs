using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CrewSpec : Specification<Crew>
{
    public CrewSpec()
    {
        Query.OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName);
    }
}