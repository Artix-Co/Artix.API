namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Museum;
using Season;

public class MusicTrack : BaseEntity
{
    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Url { get; set; }

    public bool? IsFree { get; set; }

    public long? SeasonId { get; set; }
    public virtual Season? Season { get; set; }

    public long MuseumObjectId { get; set; }
    public virtual MuseumObject MuseumObject { get; set; }


    private readonly List<UserTrack> _tracks = new(); // Renamed to _tracks
    public virtual IReadOnlyCollection<UserTrack> Tracks => _tracks.AsReadOnly();
}
