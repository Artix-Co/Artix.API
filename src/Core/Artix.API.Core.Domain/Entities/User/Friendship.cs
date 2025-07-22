

namespace Artix.API.Core.Domain.Entities.User;

using _primitives;

public sealed class Friendship : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }
    
    
    public long FriendId { get; private set; }
    public AppUser Friend { get; private set; }

    public void AssignUsers(AppUser user, AppUser friend)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Friend = friend ?? throw new ArgumentNullException(nameof(friend));
        UserId = user.Id;
        FriendId = friend.Id;
        SetModified();
    }
}
