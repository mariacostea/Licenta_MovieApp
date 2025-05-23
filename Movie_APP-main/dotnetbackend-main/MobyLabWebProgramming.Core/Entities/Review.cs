namespace MobyLabWebProgramming.Core.Entities
{
    public class Review : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid MovieId { get; set; }
        
        public Movie Movie { get; set; } = null!;

        public string Content { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}