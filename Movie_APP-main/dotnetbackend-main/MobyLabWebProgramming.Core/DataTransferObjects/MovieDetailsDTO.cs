public class MovieDetailsDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; } = default!;
    public int? Year { get; set; }
    public string? Description { get; set; }
    public double AverageRating { get; set; }
    public string? PosterUrl { get; set; }
    
    public int TmdbId { get; set; }

    public List<string> Genres { get; set; } = new();
}