using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

public sealed class UserEmailTokenSpec : Specification<User>
{
    public UserEmailTokenSpec(string token) =>
        Query.Where(u => u.EmailConfirmationToken == token);
}