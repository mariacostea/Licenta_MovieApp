using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CrewController(ICrewService crewService) : ControllerBase
{
    [HttpPost("add")]
    public async Task<IActionResult> AddCrew([FromBody] CrewDTO dto)
    {
        var result = await crewService.AddCrew(dto);
        return this.FromServiceResponse(result);
    }

    [HttpPost("assign/{movieId:guid}")]
    public async Task<IActionResult> AssignToMovie(Guid movieId, [FromBody] MovieCrewDTO dto)
    {
        var result = await crewService.AddCrewToMovie(movieId, dto);
        return this.FromServiceResponse(result);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllCrew()
    {
        var result = await crewService.GetAllCrew();
        return this.FromServiceResponse(result);
    }

    [HttpGet("movie/{movieId:guid}")]
    public async Task<IActionResult> GetMovieCrew(Guid movieId)
    {
        var result = await crewService.GetCrewForMovie(movieId);
        return this.FromServiceResponse(result);
    }
}