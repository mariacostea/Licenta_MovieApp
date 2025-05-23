using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CrewProjectionSpec : Specification<Crew, CrewDTO>
{
    public CrewProjectionSpec()
    {
        Query.OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName);
       Query.Select(c => new CrewDTO
            {
                Id        = c.Id,
                FirstName = c.FirstName,
                LastName  = c.LastName,
                ImageUrl  = c.ImageUrl,
                Birthday  = c.Birthday
            });
    }
}