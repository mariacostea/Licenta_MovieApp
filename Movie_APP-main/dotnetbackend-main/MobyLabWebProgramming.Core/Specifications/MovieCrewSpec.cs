using System;
using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class MovieCrewSpec : Specification<MovieCrew>
{
    public MovieCrewSpec(Guid movieId, Guid crewId)
        => Query.Where(mc => mc.MovieId == movieId && mc.CrewId == crewId);
}