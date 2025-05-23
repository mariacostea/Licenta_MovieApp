namespace MobyLabWebProgramming.Core.DataTransferObjects
{
    public class FriendshipDTO
    {
        public Guid Id { get; set; }

        public Guid RequesterId { get; set; }
        public string? RequesterName { get; set; }

        public Guid AddresseeId { get; set; }
        public string? AddresseeName { get; set; }

        public string Status { get; set; } = null!;
        public DateTime RequestedAt { get; set; }
        public DateTime? AcceptedAt { get; set; }
    }
}