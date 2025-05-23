namespace MobyLabWebProgramming.Core.DataTransferObjects;

public sealed class FriendshipRequestDTO
{
    public Guid FromUserId { get; init; }
    public Guid ToUserId   { get; init; }
}