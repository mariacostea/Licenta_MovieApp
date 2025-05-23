using System;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

public class FeedbackCreateDTO
{
    public string FeedbackType { get; set; } = default!;
    public int Rating { get; set; }
    public bool AgreeToTerms { get; set; }
    public string Comment { get; set; } = default!;
}