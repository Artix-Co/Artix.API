namespace Artix.API.Core.Domain.Entities.User;

using _primitives;
using Season;

public class UserSeasonProgress : BaseEntity
{
    public long UserId { get; private set; }
    public long SeasonId { get; private set; }
    public int TotalXp { get; private set; }
    public DateTime LastUpdated { get; private set; }

    public virtual Season Season { get; private set; }
    public virtual AppUser User { get; private set; }

    public UserSeasonProgress(long userId, long seasonId, Season season, AppUser user)
    {
        UserId = userId;
        SeasonId = seasonId;
        Season = season ?? throw new ArgumentNullException(nameof(season));
        User = user ?? throw new ArgumentNullException(nameof(user));
        TotalXp = 0;
        LastUpdated = DateTime.UtcNow;
    }

    public void AddXp(int xp)
    {
        if (xp <= 0) return;
        TotalXp += xp;
        LastUpdated = DateTime.UtcNow;
        SetModified();
    }

    public void UpdateLastUpdated(DateTime dateTime)
    {
        LastUpdated = dateTime;
        SetModified();
    }
}
