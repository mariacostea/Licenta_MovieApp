namespace MobyLabWebProgramming.Core.DataTransferObjects.Feed;

public class WatchedMovieDTO
{
    public string MovieTitle { get; set; } = default!;
    public string? PosterUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Description { get; set; }
    public double AverageRating { get; set; }
    public string FriendName { get; set; } = default!;
    public DateTime WatchedAt { get; set; }
}