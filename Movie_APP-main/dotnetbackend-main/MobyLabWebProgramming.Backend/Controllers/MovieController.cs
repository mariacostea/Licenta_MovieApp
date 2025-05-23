using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using System.Net;

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class MovieController(IMovieService service) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> All([FromQuery] PageDTO p)
        => Ok(ServiceResponse.ForSuccess(
            await service.GetAllMoviesAsync(p.Page, p.PageSize)));

    /* title & year exact */
    [HttpGet("by-title")]
    public async Task<IActionResult> ByTitle(
        [FromQuery] string title, [FromQuery] int year)
    {
        var m = await service.GetMovieByTitleAsync(title, year);
        return m is null
            ? NotFound(ServiceResponse.FromError<MovieDetailsDTO>(
                new ErrorMessage(HttpStatusCode.NotFound,
                                 "Film inexistent", ErrorCodes.EntityNotFound)))
            : Ok(ServiceResponse.ForSuccess(m));
    }

    /* genre */
    [HttpGet("by-genre")]
    public async Task<IActionResult> ByGenre(
        [FromQuery] string genre, [FromQuery] PageDTO p)
        => Ok(ServiceResponse.ForSuccess(
            await service.GetMoviesByGenreAsync(genre, p.Page, p.PageSize)));

    /* year */
    [HttpGet("by-year")]
    public async Task<IActionResult> ByYear(
        [FromQuery] int year, [FromQuery] PageDTO p)
        => Ok(ServiceResponse.ForSuccess(
            await service.GetMoviesByYearAsync(year, p.Page, p.PageSize)));

    /* simple search în titlu (toate rezultatele) */
    [HttpGet("search-by-title")]
    public async Task<IActionResult> SearchByTitle([FromQuery] string title)
        => Ok(ServiceResponse.ForSuccess(
            await service.SearchAllMoviesByTitleAsync(title)));

    /* filtrare compusă year + genre (ambele opţionale) */
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] int? year,
        [FromQuery] string? genre,
        [FromQuery] PageDTO p)
    {
        if (year is null && string.IsNullOrWhiteSpace(genre))
            return BadRequest(new { error = "Trimite year și/sau genre." });

        var r = await service.FilterMoviesAsync(year, genre, p.Page, p.PageSize);
        return Ok(ServiceResponse.ForSuccess(r));
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var movie = await service.GetMovieByIdAsync(id);
        return movie is not null
            ? Ok(ServiceResponse.ForSuccess(movie))
            : NotFound(ServiceResponse.FromError<MovieDetailsDTO>(
                new ErrorMessage(HttpStatusCode.NotFound,
                    "Film inexistent", ErrorCodes.EntityNotFound)));
    }

}
