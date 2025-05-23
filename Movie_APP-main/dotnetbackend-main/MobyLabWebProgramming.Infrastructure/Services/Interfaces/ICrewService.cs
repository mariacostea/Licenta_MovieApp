global using MobyLabWebProgramming.Core.Responses;
global using MobyLabWebProgramming.Core.DataTransferObjects;
global using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

public interface ICrewService
{
    Task<ServiceResponse> AddCrew(CrewDTO dto, CancellationToken ct = default);
    Task<ServiceResponse<List<CrewDTO>>> GetAllCrew(CancellationToken ct = default);
    Task<ServiceResponse> AddCrewToMovie(Guid movieId, MovieCrewDTO dto, CancellationToken ct = default);
    Task<ServiceResponse<List<CrewDTO>>> GetCrewForMovie(Guid movieId, CancellationToken ct = default);
}
