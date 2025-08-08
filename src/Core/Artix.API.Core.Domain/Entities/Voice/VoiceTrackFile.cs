namespace Artix.API.Core.Domain.Entities.Voice;

using Artix.API.Core.Domain.Entities.User;
using File;

public class VoiceTrackFile
{
    public long FileId { get; private set; }
    public virtual File File { get; private set; }

    public long VoiceTrackId { get; private set; }
    public virtual VoiceTrack VoiceTrack { get; private set; }
}
