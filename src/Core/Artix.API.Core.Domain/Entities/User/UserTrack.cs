namespace Artix.API.Core.Domain.Entities.User;

using _primitives;
using MusicTrack;

public class UserTrack : BaseEntity
{
    public long UserId { get; private set; }
    public long TrackId { get; private set; }
    public DateTime AcquiredAt { get; private set; }

    public virtual MusicTracks Track { get; private set; }
    public virtual AppUser User { get; private set; }

    public UserTrack(long userId, long trackId, MusicTracks track, AppUser user, DateTime acquiredAt)
    {
        UserId = userId;
        TrackId = trackId;
        Track = track ?? throw new ArgumentNullException(nameof(track));
        User = user ?? throw new ArgumentNullException(nameof(user));
        AcquiredAt = acquiredAt;
    }

    public void UpdateAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
        SetModified();
    }
}



