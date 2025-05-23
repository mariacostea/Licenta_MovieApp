namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ReviewDTO
{
    public Guid MovieId { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
    public string? Author { get; set; } // numele complet al userului
    public Guid UserId { get; set; } // pentru identificare în frontend
    public Guid Id { get; set; } // pentru edit/delete
}
