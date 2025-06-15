using System.Net;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class EventService : IEventService
{
    private readonly IRepository<WebAppDatabaseContext> _repo;

    public EventService(IRepository<WebAppDatabaseContext> repo) => _repo = repo;

    // CREATE
    public async Task<EventDTO> CreateEventAsync(EventCreateDTO dto, Guid userId, UserRoleEnum role)
    {
        if (role == UserRoleEnum.User && dto.MaxParticipants > 10)
        {
            throw new ServerException(HttpStatusCode.BadRequest, "Userul standard poate crea evenimente cu maxim 10 participanți.");
        }

        var movie = await _repo.GetAsync<Movie>(dto.MovieId);
        if (movie is null)
            throw new ServerException(HttpStatusCode.BadRequest, "Filmul nu există în baza de date.");

        var newEvent = new Event
        {
            Title = dto.Title,
            Description = dto.Description,
            Location = dto.Location,
            Date = dto.Date,
            MaxParticipants = dto.MaxParticipants,
            FreeSeats = dto.MaxParticipants,
            OrganizerId = userId,
            MovieId = dto.MovieId
        };

        await _repo.AddAsync(newEvent);
        
        var userEvent = new UserEvent
        {
            UserId = userId,
            EventId = newEvent.Id,
            Status = "Organizer",
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(userEvent);

        return new EventDTO
        {
            Id = newEvent.Id,
            Title = newEvent.Title,
            Description = newEvent.Description,
            Location = newEvent.Location,
            Date = newEvent.Date,
            MaxParticipants = newEvent.MaxParticipants,
            FreeSeats = newEvent.FreeSeats,
            OrganizerId = userId,
            MovieId = newEvent.MovieId,
            CreatedAt = newEvent.CreatedAt,
            MovieTitle = movie.Title,
            MoviePosterUrl = movie.PosterUrl
        };
    }

    // UPDATE
    public async Task<EventDTO> UpdateEventAsync(Guid eventId, EventCreateDTO dto, Guid userId)
    {
        var isOrganizer = await _repo.AnyAsync(new UserEventIsOrganizerSpec(eventId, userId));
        if (!isOrganizer)
            throw new ServerException(HttpStatusCode.Forbidden, "Access denied. You must be the organizer to update this event.");
        
        var ev = await _repo.GetAsync(new EventSpec(eventId, includeDetails: true));
        if (ev == null)
            throw new ServerException(HttpStatusCode.NotFound, "Event not found.");
        
        var movie = await _repo.GetAsync<Movie>(dto.MovieId);
        if (movie == null)
            throw new ServerException(HttpStatusCode.BadRequest, "Filmul selectat nu există.");
        
        if (dto.MaxParticipants > 10)
            throw new ServerException(HttpStatusCode.BadRequest, "Userul standard poate seta maxim 10 participanți.");

        ev.Title = dto.Title;
        ev.Description = dto.Description;
        ev.Location = dto.Location;
        ev.Date = dto.Date;
        ev.MovieId = dto.MovieId; 
        
        if (ev.FreeSeats == ev.MaxParticipants)
        {
            ev.MaxParticipants = dto.MaxParticipants;
            ev.FreeSeats = dto.MaxParticipants;
        }
        
        await _repo.UpdateAsync(ev);

        return new EventDTO
        {
            Id = ev.Id,
            Title = ev.Title,
            Description = ev.Description,
            Location = ev.Location,
            Date = ev.Date,
            MaxParticipants = ev.MaxParticipants,
            FreeSeats = ev.FreeSeats,
            OrganizerId = ev.OrganizerId,
            MovieId = ev.MovieId,
            CreatedAt = ev.CreatedAt,
            MovieTitle = movie.Title,
            MoviePosterUrl = movie.PosterUrl
        };
    }


    // DELETE
    public async Task DeleteEventAsync(Guid eventId, Guid userId)
    {
        try
        {
            var isOrganizer = await _repo.AnyAsync(new UserEventIsOrganizerSpec(eventId, userId));

            if (!isOrganizer)
                throw new ServerException(HttpStatusCode.Forbidden, "Access denied. You must be the organizer to delete this event.");

            await _repo.DeleteAsync<Event>(eventId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in DeleteEventAsync: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            throw;
        }
    }




    // LIST METHODS
       public async Task<List<EventDTO>> GetEventsByUserRoleAsync(Guid userId, string role)
    {
        return await _repo.ListAsync(new EventProjectionSpec(userId, role));
    }

    public async Task<List<EventDTO>> GetEventsByLocationAsync(string location) =>
        await _repo.ListAsync(new EventProjectionSpec(location));

    public async Task<List<EventDTO>> GetEventsByDayAsync(DateTime date) =>
        await _repo.ListAsync(new EventProjectionSpec(date.Date, date.Date.AddDays(1)));

    public async Task<List<EventDTO>> GetEventsByFullDateTimeAsync(DateTime dateTime) =>
        await _repo.ListAsync(new EventProjectionSpec(dateTime, dateTime));

    public async Task<List<EventDTO>> GetEventsByMonthAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddTicks(-1);
        return await _repo.ListAsync(new EventProjectionSpec(start, end));
    }

    public async Task<List<EventDTO>> GetEventsByMovieIdAsync(Guid movieId) =>
        await _repo.ListAsync(new EventProjectionSpec(movieId));

    public async Task<EventDTO?> GetEventByIdAsync(Guid id) =>
        await _repo.GetAsync(new EventProjectionSpec(id));

    public async Task<List<EventDTO>> GetEventsByMovieTitleAsync(string title)
    {
        return await _repo.ListAsync(new EventByMovieTitleSpec(title));
    }

    public async Task<List<EventDTO>> GetAllEventsAsync()
    {
        return await _repo.ListAsync(new EventProjectionSpec());
    }

    public async Task AttendEventAsync(Guid eventId, Guid userId)
    {
        var ev = await _repo.GetAsync<Event>(eventId);
        if (ev == null)
            throw new ServerException(HttpStatusCode.NotFound, "Event not found.");
        
        if (await HasUserAlreadyJoined(eventId, userId))
            throw new ServerException(HttpStatusCode.BadRequest, "You have already joined this event.");
        
        if (ev.FreeSeats <= 0)
            throw new ServerException(HttpStatusCode.BadRequest, "No seats available for this event.");
        
        var userEvent = new UserEvent
        {
            UserId = userId,
            EventId = eventId,
            Status = "Participant",
            CreatedAt = DateTime.UtcNow
        };
        
        await _repo.AddAsync(userEvent);
        
        ev.FreeSeats--;
        await _repo.UpdateAsync(ev);
    }

    private async Task<bool> HasUserAlreadyJoined(Guid eventId, Guid userId)
    {
        return await _repo.AnyAsync(new UserEventSpec(eventId, userId));
    }

    
    public async Task<List<EventDTO>> GetEventsByUserIdAsync(Guid userId)
    {
        return await _repo.ListAsync(new EventProjectionSpec(userId, "Participant"));
    }
    
    public async Task<List<EventDTO>> GetAllEventsUnatendedAsync(Guid userId)
    {
        return await _repo.ListAsync(new EventProjectionSpec(userId, "Other"));
    }

    public async Task<List<EventDTO>> GetEventsUserIsOrganizerAsync(Guid userId)
    {
        return await _repo.ListAsync(new EventProjectionSpec(userId, "Organizer"));
    }

}
