using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.Entities;

public class MovieCrew : BaseEntity
{
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; } = default!;

    public Guid CrewId { get; set; }
    public Crew Crew { get; set; } = default!;

    public PersonTypeEnum PersonType { get; set; }
}