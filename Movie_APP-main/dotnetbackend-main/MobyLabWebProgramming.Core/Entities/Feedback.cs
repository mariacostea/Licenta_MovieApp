using System;

namespace MobyLabWebProgramming.Core.Entities;

public class Feedback : BaseEntity
{
    public string FeedbackType { get; set; } = default!;
    public int Rating { get; set; }
    public bool AgreeToTerms { get; set; }
    public string Comment { get; set; } = default!;
    public Guid UserId { get; set; }

    public User? User { get; set; }
}