using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Entities
{
    [Index(nameof(UserId), nameof(MovieId), IsUnique = true)]
    public class UserMovie : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = default!;

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; } = default!;

        public bool IsWatched { get; set; }
        public DateTime WatchedOn { get; set; }
    }

}