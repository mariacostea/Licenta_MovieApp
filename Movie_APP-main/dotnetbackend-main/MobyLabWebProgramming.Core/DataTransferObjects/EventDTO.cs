namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class EventDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Location { get; set; } = null!;
    public DateTime Date { get; set; }
    public int MaxParticipants { get; set; }
    public int FreeSeats { get; set; }
    public Guid OrganizerId { get; set; }
    public Guid MovieId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string? MovieTitle { get; set; }
    
    public string? MoviePosterUrl { get; set; }

}