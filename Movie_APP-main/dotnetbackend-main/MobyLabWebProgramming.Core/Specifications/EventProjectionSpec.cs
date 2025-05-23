using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class EventProjectionSpec : Specification<Event, EventDTO>
{
    public EventProjectionSpec(bool orderByCreatedAt = false)
    {
        // Include Movie pentru a avea acces la Title si PosterUrl
        Query.Include(e => e.Movie);

        Query.Select(e => new EventDTO
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Location = e.Location,
            Date = e.Date,
            MaxParticipants = e.MaxParticipants,
            FreeSeats = e.FreeSeats,
            OrganizerId = e.OrganizerId,
            MovieId = e.MovieId,
            CreatedAt = e.CreatedAt,

            // Acestea sunt luate acum din Movie (join automat facut de EF)
            MovieTitle = e.Movie != null ? e.Movie.Title : "Unknown",
            MoviePosterUrl = e.Movie != null ? e.Movie.PosterUrl : null
        });

        if (orderByCreatedAt)
        {
            Query.OrderByDescending(e => e.CreatedAt);
        }
    }

    public EventProjectionSpec(Guid id) : this()
    {
        Query.Where(e => e.Id == id);
    }

    public EventProjectionSpec(string? search) : this(true)
    {
        if (string.IsNullOrWhiteSpace(search)) return;

        var keyword = $"%{search.Trim().Replace(" ", "%")}%" ;

        Query.Where(e =>
            EF.Functions.ILike(e.Title, keyword) ||
            EF.Functions.ILike(e.Description ?? "", keyword) ||
            EF.Functions.ILike(e.Location, keyword));
    }

    public EventProjectionSpec(DateTime date) : this()
    {
        Query.Where(e => e.Date.Date == date.Date);
    }

    public EventProjectionSpec(Guid movieId, bool byMovie = true) : this()
    {
        if (byMovie)
        {
            Query.Where(e => e.MovieId == movieId);
        }
    }

    public EventProjectionSpec(DateTime from, DateTime to) : this()
    {
        Query.Where(e => e.Date >= from && e.Date <= to);
    }
    
    public EventProjectionSpec(Guid userId, string userRole) : this()
    {
        Query.Include(e => e.Movie)
            .Include(e => e.UserEvents);

        if (userRole == "Organizer")
        {
            Query.Where(e => e.OrganizerId == userId);
        }
        else if (userRole == "Participant")
        {
            Query.Where(e => e.UserEvents.Any(ue => ue.UserId == userId && ue.Status == "Participant"));
        }
        else if (userRole == "Other")
        {
            Query.Where(e => e.OrganizerId != userId && !e.UserEvents.Any(ue => ue.UserId == userId && ue.Status == "Participant"));
        }
    }



}
