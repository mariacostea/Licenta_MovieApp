using System;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieCrewWithCrewProjectionSpec : Specification<MovieCrew, CrewDTO>
{
    public MovieCrewWithCrewProjectionSpec(Guid movieId)
    {
        Query.Where(mc => mc.MovieId == movieId)
            .Include(mc => mc.Crew);
            
        Query.Select(mc => new CrewDTO
            {
                Id        = mc.Crew.Id,
                FirstName = mc.Crew.FirstName,
                LastName  = mc.Crew.LastName,
                ImageUrl  = mc.Crew.ImageUrl,
                Birthday  = mc.Crew.Birthday
            });
    }
}