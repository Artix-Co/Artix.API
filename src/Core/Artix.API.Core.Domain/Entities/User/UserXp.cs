namespace Artix.API.Core.Domain.Entities.User;

using _primitives;

public class UserXp : BaseEntity
{
    public long UserId { get; private set; }
    public long TotalXp { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public virtual AppUser User { get; private set; }

    public UserXp(long userId, AppUser user)
    {
        UserId = userId;
        User = user ?? throw new ArgumentNullException(nameof(user));
        TotalXp = 0;
        LastUpdated = DateTime.UtcNow;
    }

    public void AddXp(long xp)
    {
        if (xp <= 0) return;
        TotalXp += xp;
        LastUpdated = DateTime.UtcNow;
        SetModified();
    }

    public void UpdateLastUpdated(DateTime lastUpdated)
    {
        LastUpdated = lastUpdated;
        SetModified();
    }
}
