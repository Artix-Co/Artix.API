namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Season;

public sealed class UserSeasonProgress : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }
    
    
    public long SeasonId { get; private set; }
    public Season Season { get; private set; }
    
    
    public int TotalXp { get; private set; }
    public DateTime LastUpdated { get; private set; }

  
 

     

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
