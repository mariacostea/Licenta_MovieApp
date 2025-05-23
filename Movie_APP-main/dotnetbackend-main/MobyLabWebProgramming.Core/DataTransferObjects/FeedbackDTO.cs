using System;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class FeedbackDTO
{
    public Guid Id { get; set; }
    public string FeedbackType { get; set; } = default!;
    public int Rating { get; set; }
    public bool AgreeToTerms { get; set; }
    public string Comment { get; set; } = default!;
    public Guid UserId { get; set; }
}