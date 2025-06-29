namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class UserExtendedProfileDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Email { get; set; }
    public string? ProfilePictureUrl { get; set; }

    public int WatchedCount { get; set; }
    public int RecommendedCount { get; set; }

    public List<MovieSimpleDTO> WatchedMovies { get; set; } = new();
    public List<MovieSimpleDTO> RecommendedMovies { get; set; } = new();
    public List<EventDTO> OrganizedEvents { get; set; } = new();
}

public class MovieSimpleDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? PosterUrl { get; set; }
}

public class EventSimpleDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}

