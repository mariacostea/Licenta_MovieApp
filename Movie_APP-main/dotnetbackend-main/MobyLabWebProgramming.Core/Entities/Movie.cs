namespace MobyLabWebProgramming.Core.Entities;

public class Movie : BaseEntity
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int? Year { get; set; }
    
    public int TmdbId { get; set; }
    
    public double AverageRating { get; set; }

    public string? PosterUrl { get; set; }

    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<UserMovie> UserMovies { get; set; } = new List<UserMovie>();
    public ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
}
