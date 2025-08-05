namespace Artix.API.Core.Domain.Entities.User;

using Common;

public class UserTrack : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }


    public long TrackId { get; private set; }
    public virtual MusicTrack Track { get; private set; }


    public DateTime AcquiredAt { get; private set; }


    public void UpdateAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
    }
}
