namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class EventCreateDTO
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Location { get; set; } = null!;
    public DateTime Date { get; set; }
    public int MaxParticipants { get; set; }
    public Guid MovieId { get; set; }
}

