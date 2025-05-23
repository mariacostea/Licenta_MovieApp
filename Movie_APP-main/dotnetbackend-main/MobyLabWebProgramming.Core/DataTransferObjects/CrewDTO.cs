namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class CrewDTO
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string? ImageUrl { get; set; } = default!;
    public DateTime? Birthday { get; set; }
}