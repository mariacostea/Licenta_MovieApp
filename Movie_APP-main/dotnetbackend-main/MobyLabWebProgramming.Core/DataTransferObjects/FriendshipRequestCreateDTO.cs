namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class FriendshipRequestCreateDTO
{
    public Guid ToUserId { get; set; }
    
    public string? PhotoUrlRequester { get; set; }
    public string? PhotoUrlAddressee { get; set; }
}