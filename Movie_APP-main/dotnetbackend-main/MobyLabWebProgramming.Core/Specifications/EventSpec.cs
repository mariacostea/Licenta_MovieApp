using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class EventSpec : Specification<Event>
{
    public EventSpec(Guid id, bool includeDetails = false)
    {
        Query.Where(e => e.Id == id);

        if (includeDetails)
        {
            Query.Include(e => e.Organizer)
                .Include(e => e.UserEvents)
                .ThenInclude(ue => ue.User);
        }
    }
    
    public EventSpec(string value, bool searchByLocation = false)
    {
        if (searchByLocation)
            Query.Where(e => e.Location == value);
        else
            Query.Where(e => e.Title == value);
    }
    
    public EventSpec(DateTime from, DateTime to)
    {
        Query.Where(e => e.Date >= from && e.Date <= to);
    }
    
    public EventSpec(DateOnly date)
    {
        var start = date.ToDateTime(TimeOnly.MinValue);
        var end = date.ToDateTime(TimeOnly.MaxValue);

        Query.Where(e => e.Date >= start && e.Date <= end);
    }
    
    public EventSpec(DateTime exactDateTime)
    {
        Query.Where(e => e.Date == exactDateTime);
    }
    
    public EventSpec(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1).AddTicks(-1);

        Query.Where(e => e.Date >= start && e.Date <= end);
    }
    
    public EventSpec(Guid movieId)
    {
        Query.Where(e => e.MovieId == movieId);
    }
}