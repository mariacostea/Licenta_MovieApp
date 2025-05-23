using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class CrewByTmdbIdSpec : Specification<Crew>
{
    public CrewByTmdbIdSpec(int tmdbId) => Query.Where(c => c.TmdbId == tmdbId);
}