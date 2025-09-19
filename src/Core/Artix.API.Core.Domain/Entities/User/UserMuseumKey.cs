

namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;
using Museum;

public class UserMuseumKey : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }
    public long MuseumId { get; private set; }  // موزه مربوطه
    public virtual Museum Museum { get; private set; }
    public bool IsUnlocked { get; private set; } = false;  // آیا کامل شده؟
    public DateTime? UnlockedAt { get; private set; }
    public int ShareCount { get; private set; } = 0;  // تعداد shareها برای tracking engagement
    public bool IsShared { get; private set; } = false;  // آیا share شده؟
    

    protected UserMuseumKey() { }

    private UserMuseumKey(long userId, long museumId)
    {
        UserId = userId;
        MuseumId = museumId;
    }

    public static UserMuseumKey Create(long userId, long museumId)
    {
        return new UserMuseumKey(userId, museumId);
    }

    public void Unlock(DateTime unlockedAt)
    {
        if (IsUnlocked) return;
        IsUnlocked = true;
        UnlockedAt = unlockedAt;
        // Raise event برای XP award یا tier recalc
    }

    public void ShareWithFriend(long friendUserId)
    {
        ShareCount++;
        IsShared = true;
        // Logic برای notify friend و award XP
    }
}
