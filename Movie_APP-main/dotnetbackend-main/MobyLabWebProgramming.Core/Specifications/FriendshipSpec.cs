using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MobyLabWebProgramming.Core.Specifications
{
    public sealed class FriendshipSpec : Specification<Friendship>
    {
        public FriendshipSpec(bool includeUsers = false)
        {
            if (includeUsers)
            {
                Query
                    .Include(f => f.Requester)
                    .Include(f => f.Addressee);
            }
        }

        public FriendshipSpec(Guid id, bool includeUsers = false) : this(includeUsers)
        {
            Query.Where(f => f.Id == id);
        }
        
        public FriendshipSpec(Guid userId, bool filterByUser, bool includeUsers = false) : this(includeUsers)
        {
            Query.Where(f => f.RequesterId == userId || f.AddresseeId == userId);
        }
        
        public FriendshipSpec(string status, bool byStatus, bool includeUsers = false) : this(includeUsers)
        {
            if (!string.IsNullOrWhiteSpace(status))
            {
                Query.Where(f => f.Status.ToString() == status);
            }
        }
    }
}