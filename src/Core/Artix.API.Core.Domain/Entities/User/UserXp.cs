namespace Artix.API.Core.Domain.Entities.User;

using Common;

public class UserXp : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }


    public long TotalXp { get; private set; }
    public DateTime LastUpdated { get; private set; }


    public void AddXp(long xp)
    {
        if (xp <= 0) return;
        TotalXp += xp;
        LastUpdated = DateTime.UtcNow;
    }

    public void UpdateLastUpdated(DateTime lastUpdated)
    {
        LastUpdated = lastUpdated;
    }
}
