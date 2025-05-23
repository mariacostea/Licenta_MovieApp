// Core/Entities/Crew.cs
namespace MobyLabWebProgramming.Core.Entities;

public class Crew : BaseEntity
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? ImageUrl { get; set; }
    public DateTime? Birthday { get; set; }
    public int? TmdbId { get; set; }

    public ICollection<MovieCrew> MovieCrews { get; set; } = new List<MovieCrew>();
}
