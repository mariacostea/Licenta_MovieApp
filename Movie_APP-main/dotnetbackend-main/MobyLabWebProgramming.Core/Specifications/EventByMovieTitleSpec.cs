using Ardalis.Specification;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

public sealed class EventByMovieTitleSpec : Specification<Event, EventDTO>
{
    public EventByMovieTitleSpec(string title)
    {
        Query
            .Include(e => e.Movie)
            .Where(e => e.Movie != null && e.Movie.Title.ToLower().Contains(title.ToLower()));
            
        Query
            .Select(e => new EventDTO
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
                CreatedAt = e.CreatedAt
            });
    }
}