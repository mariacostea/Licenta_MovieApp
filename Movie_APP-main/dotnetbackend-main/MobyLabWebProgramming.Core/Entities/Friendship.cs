namespace MobyLabWebProgramming.Core.Entities
{
    public class Friendship : BaseEntity
    {
        public Guid RequesterId  { get; set; }
        public Guid AddresseeId  { get; set; }

        public FriendshipStatus Status      { get; set; } = FriendshipStatus.Pending;
        public DateTime RequestedAt         { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt         { get; set; }

        public User Requester  { get; set; } = default!;
        public User Addressee  { get; set; } = default!;
        
    }
}