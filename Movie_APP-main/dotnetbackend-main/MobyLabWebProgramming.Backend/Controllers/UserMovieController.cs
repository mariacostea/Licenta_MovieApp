using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Net;
using System.Security.Claims;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UserMovieController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IRepository<WebAppDatabaseContext> _userMovieRepo;

    public UserMovieController(IMovieService movieService, IRepository<WebAppDatabaseContext> userMovieRepo)
    {
        _movieService = movieService;
        _userMovieRepo = userMovieRepo;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> MarkAsWatched([FromBody] MovieDTO movieDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(movieDto.Title) || movieDto.Year is null)
            {
                return BadRequest(ServiceResponse.FromError(new ErrorMessage(
                    HttpStatusCode.BadRequest,
                    "Titlul și anul filmului trebuie specificate.",
                    ErrorCodes.InvalidValue)));
            }

            var movie = await _movieService.GetMovieByTitleAsync(movieDto.Title, movieDto.Year.Value);

            if (movie == null)
            {
                return NotFound(ServiceResponse.FromError(new ErrorMessage(
                    HttpStatusCode.NotFound,
                    "Filmul nu există în baza de date.",
                    ErrorCodes.EntityNotFound)));
            }

            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var userMovie = new UserMovie
            {
                UserId = userId,
                MovieId = movie.Id,
                IsWatched = true,
                WatchedOn = DateTime.UtcNow
            };

            await _userMovieRepo.AddAsync(userMovie);

            return Ok(ServiceResponse.ForSuccess(new { message = "Filmul a fost marcat ca vizionat.", movieId = movie.Id }));
        }
        catch (Exception ex)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, ServiceResponse.FromError(
                new ErrorMessage(HttpStatusCode.InternalServerError, $"A apărut o eroare: {ex.Message}", ErrorCodes.TechnicalError)));
        }
    }

    [HttpGet("watched")]
    [Authorize]
    public async Task<IActionResult> GetWatchedMovies()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var watchedMovieIds = await _userMovieRepo.GetAsQueryable<UserMovie>()
            .Where(um => um.UserId == userId && um.IsWatched)
            .Select(um => um.MovieId)
            .ToListAsync();

        return Ok(ServiceResponse.ForSuccess(watchedMovieIds));
    }
    
}
