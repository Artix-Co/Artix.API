namespace Artix.API.Core.Domain.Entities.Voice;

using Common;
using Museum;
using Object;
using Season;

public class VoiceTrack : AggregateRoot
{
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public bool? IsFree { get; set; }
    public long? SeasonId { get; set; }
    public virtual Season? Season { get; set; }
    public long ObjectId { get; set; }
    public virtual Object Object { get; set; }
    
    private readonly List<VoiceTrackFile> _voiceTrackFiles = new();
    public virtual IReadOnlyCollection<VoiceTrackFile> VoiceTrackFiles => this._voiceTrackFiles.AsReadOnly();
}
