using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Net;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] ReviewDTO dto)
    {
        try
        {
            if (dto.Rating < 1 || dto.Rating > 10)
            {
                throw new ServerException(
                    ErrorCodeMappings.CodeToStatus[ErrorCodes.InvalidValue],
                    "Ratingul trebuie să fie între 1 și 10.",
                    ErrorCodes.InvalidValue);
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _reviewService.AddReviewAsync(dto, userId);
            return Ok(ServiceResponse.ForSuccess("Review adăugat cu succes."));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la adăugarea review-ului: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
        
    }

    [HttpPut("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid reviewId, [FromBody] ReviewDTO dto)
    {
        try
        {
            if (dto.Rating < 1 || dto.Rating > 10)
            {
                throw new ServerException(
                    ErrorCodeMappings.CodeToStatus[ErrorCodes.InvalidValue],
                    "Ratingul trebuie să fie între 1 și 10.",
                    ErrorCodes.InvalidValue);
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _reviewService.UpdateReviewAsync(reviewId, dto, userId);
            return Ok(ServiceResponse.ForSuccess("Review actualizat cu succes."));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la actualizarea review-ului: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }

    [HttpDelete("{reviewId}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid reviewId)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _reviewService.DeleteReviewAsync(reviewId, userId);
            return Ok(ServiceResponse.ForSuccess("Review șters cu succes."));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la ștergerea review-ului: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetByMovieTitleAndYear([FromQuery] string title, [FromQuery] int year)
    {
        try
        {
            var reviews = await _reviewService.GetReviewsByMovieTitleAndYearAsync(title, year);
            return Ok(ServiceResponse.ForSuccess(reviews));
        }
        catch (ServerException ex)
        {
            return StatusCode((int)ex.Status, ServiceResponse.FromError<List<ReviewDTO>>(new ErrorMessage(
                ex.Status,
                ex.Message,
                ex.Code)));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError,
                ServiceResponse.FromError<List<ReviewDTO>>(new ErrorMessage(
                    HttpStatusCode.InternalServerError,
                    $"Eroare la obținerea review-urilor: {ex.Message}",
                    ErrorCodes.TechnicalError)));
        }
    }

}
