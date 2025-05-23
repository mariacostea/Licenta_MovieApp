namespace MobyLabWebProgramming.Core.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; } = null!;
    public int TmdbId { get; set; }
    
    public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
}
