namespace MobyLabWebProgramming.Core.Entities;

public class Event : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Location { get; set; } = null!;
    public DateTime Date { get; set; }
    public string? Description { get; set; }

    public int MaxParticipants { get; set; }
    public int FreeSeats { get; set; }

    public Guid OrganizerId { get; set; }
    public User Organizer { get; set; } = null!;

    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = null!;

    public ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
}
