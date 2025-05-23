using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController(IFeedbackService feedbackService) : ControllerBase
{
    private Guid GetLoggedInUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    [HttpPost("submit")]
    [Authorize]
    public async Task<ActionResult<ServiceResponse>> SubmitFeedback([FromBody] FeedbackCreateDTO dto)
    {
        var userId = GetLoggedInUserId();
        await feedbackService.SubmitFeedbackAsync(dto, userId);
        return Ok(ServiceResponse.ForSuccess());
    }


    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ServiceResponse<List<FeedbackDTO>>>> GetAll()
    {
        var list = await feedbackService.GetAllFeedbacksAsync();
        return Ok(ServiceResponse.ForSuccess(list));
    }
}