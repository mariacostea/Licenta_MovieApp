namespace MobyLabWebProgramming.Core.Entities
{
    public class UserEvent : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid EventId { get; set; }
        public Event Event { get; set; } = null!;
        public string? Status { get; set; }
    }
}