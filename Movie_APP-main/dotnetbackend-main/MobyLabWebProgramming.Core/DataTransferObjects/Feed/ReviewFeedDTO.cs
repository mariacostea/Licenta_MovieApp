namespace MobyLabWebProgramming.Core.DataTransferObjects.Feed;

public class ReviewFeedDTO
{
    public string MovieTitle { get; set; } = default!;
    public string? PosterUrl { get; set; }
    public int? ReleaseYear { get; set; }
    public string? Description { get; set; }
    public double? Rating { get; set; }

    public string FriendName { get; set; } = default!;
    public DateTime ReviewedAt { get; set; }

    public string ReviewText { get; set; } = default!;
    public int FriendRating { get; set; }
}