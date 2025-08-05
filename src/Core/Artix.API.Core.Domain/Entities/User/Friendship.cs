namespace Artix.API.Core.Domain.Entities.User;

public class Friendship
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }


    public long FriendId { get; private set; }
    public virtual AppUser Friend { get; private set; }
    public DateTime CreatedAt { get; set; }


    private void AssignUsers(AppUser user, AppUser friend)
    {
        User = user;
        Friend = friend;
        UserId = user.Id;
        FriendId = friend.Id;
    }

    public static Friendship Create(AppUser user, AppUser friend)
    {
        var friendship = new Friendship();
        friendship.AssignUsers(user, friend);
        return friendship;
    }
}
