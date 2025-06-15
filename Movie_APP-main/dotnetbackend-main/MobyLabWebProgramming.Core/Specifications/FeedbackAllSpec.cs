using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

public sealed class FeedbackAllSpec : Specification<Feedback>
{
    public FeedbackAllSpec()
    {
        Query.OrderByDescending(f => f.CreatedAt);
    }
}