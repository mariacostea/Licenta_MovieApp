namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class ReviewDTO
{
    public Guid MovieId { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
    public string? Author { get; set; }
    public Guid UserId { get; set; }
    public Guid Id { get; set; }
}
