using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using MobyLabWebProgramming.Core.DataTransferObjects;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationsService _recommendationsService;

    public RecommendationsController(IRecommendationsService recommendationsService)
    {
        _recommendationsService = recommendationsService;
    }

    [HttpGet("genre")]
    public async Task<IActionResult> GetByGenre([FromQuery] Guid userId)
    {
        var result = await _recommendationsService.GetRecommendedByGenre(userId);
        return Ok(result);
    }

    [HttpGet("actors")]
    public async Task<IActionResult> GetByActors([FromQuery] Guid userId)
    {
        var result = await _recommendationsService.GetRecommendedByActors(userId);
        return Ok(result);
    }

    [HttpGet("description")]
    public async Task<IActionResult> GetByDescription([FromQuery] Guid userId)
    {
        var result = await _recommendationsService.GetRecommendedByDescription(userId);
        return Ok(result);
    }

    [HttpGet("combined")]
    public async Task<IActionResult> GetCombined([FromQuery] Guid userId)
    {
        var result = await _recommendationsService.GetCombinedRecommendations(userId);
        return Ok(result);
    }
}