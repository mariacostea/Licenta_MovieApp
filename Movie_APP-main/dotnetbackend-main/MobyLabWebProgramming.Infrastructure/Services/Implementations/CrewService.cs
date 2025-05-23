using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class CrewService : ICrewService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public CrewService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    // ---------------------- CREATE CREW ----------------------
    public async Task<ServiceResponse> AddCrew(CrewDTO dto, CancellationToken ct = default)
    {
        var crew = new Crew
        {
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            ImageUrl  = dto.ImageUrl,
            Birthday  = dto.Birthday
        };

        await _repo.AddAsync(crew, ct);
        return ServiceResponse.ForSuccess();
    }

    // ---------------------- READ ALL CREW --------------------
    public async Task<ServiceResponse<List<CrewDTO>>> GetAllCrew(CancellationToken ct = default)
    {
        var crews = await _repo.ListAsync(new CrewSpec(), ct);

        var list = crews.Select(c => new CrewDTO
        {
            Id        = c.Id,
            FirstName = c.FirstName,
            LastName  = c.LastName,
            ImageUrl  = c.ImageUrl,
            Birthday  = c.Birthday
        }).ToList();

        return ServiceResponse<List<CrewDTO>>.ForSuccess(list);
    }

    // ------------------ LINK CREW → MOVIE --------------------
    public async Task<ServiceResponse> AddCrewToMovie(Guid movieId, MovieCrewDTO dto, CancellationToken ct = default)
    {
        var link = new MovieCrew
        {
            MovieId    = movieId,
            CrewId     = dto.CrewId,
            PersonType = dto.PersonType
        };

        await _repo.AddAsync(link, ct);
        return ServiceResponse.ForSuccess();
    }

    // ------------- GET CREW FOR A SINGLE MOVIE ---------------
    public async Task<ServiceResponse<List<CrewDTO>>> GetCrewForMovie(Guid movieId, CancellationToken ct = default)
    {
        var movieCrew = await _repo.ListAsync(new MovieCrewWithCrewSpec(movieId), ct);

        var crewList = movieCrew.Select(mc => new CrewDTO
        {
            Id        = mc.Crew.Id,
            FirstName = mc.Crew.FirstName,
            LastName  = mc.Crew.LastName,
            ImageUrl  = mc.Crew.ImageUrl,
            Birthday  = mc.Crew.Birthday
        }).ToList();

        return ServiceResponse<List<CrewDTO>>.ForSuccess(crewList);
    }

    // -------------------- SPECIFICAȚII -----------------------
    /// <summary>Returnează toate persoanele din crew, ordonate alfabetic.</summary>
    private sealed class CrewSpec : Specification<Crew>
    {
        public CrewSpec()
        {
            Query.OrderBy(c => c.LastName)
                 .ThenBy(c => c.FirstName);
        }
    }

    /// <summary>Întoarce toate legăturile Movie-Crew pt. un film și include entitatea Crew.</summary>
    private sealed class MovieCrewWithCrewSpec : Specification<MovieCrew>
    {
        public MovieCrewWithCrewSpec(Guid movieId)
        {
            Query.Where(mc => mc.MovieId == movieId)
                 .Include(mc => mc.Crew);
        }
    }
}
