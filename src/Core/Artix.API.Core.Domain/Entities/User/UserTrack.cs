namespace Artix.API.Core.Domain.Entities.User;

using Common;

public sealed class UserTrack : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }
    
    
    public long TrackId { get; private set; }
    public MusicTrack Track { get; private set; }
    
    
    public DateTime AcquiredAt { get; private set; }


 

     
    public void UpdateAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
        SetModified();
    }
}



