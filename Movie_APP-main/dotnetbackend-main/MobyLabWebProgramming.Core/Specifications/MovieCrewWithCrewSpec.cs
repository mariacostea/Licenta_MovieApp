using System;
using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieCrewWithCrewSpec : Specification<MovieCrew>
{
    public MovieCrewWithCrewSpec(Guid movieId)
    {
        Query.Where(mc => mc.MovieId == movieId)
            .Include(mc => mc.Crew);
    }
}