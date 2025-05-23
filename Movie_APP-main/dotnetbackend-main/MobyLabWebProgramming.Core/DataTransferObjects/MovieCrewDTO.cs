using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class MovieCrewDTO
{
    public Guid CrewId { get; set; }
    public PersonTypeEnum PersonType { get; set; }
}