using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class GenreSpec : Specification<Genre>
{
    public GenreSpec(string name)
    {
        Query.Where(g => g.Name == name);
    }

    public GenreSpec(int tmdbId)
    {
        Query.Where(g => g.TmdbId == tmdbId);
    }
}