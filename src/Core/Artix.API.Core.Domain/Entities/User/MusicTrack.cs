namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Museum;
using Season;

public sealed class MusicTrack : BaseEntity
{
    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Url { get; set; }

    public bool? IsFree { get; set; }

    public long? SeasonId { get; set; }
    public Season? Season { get; set; }
    
    public long MuseumObjectId { get; set; }
    public MuseumObject MuseumObject { get; set; }
    
    private readonly List<UserTrack> _objects = new();
    public IReadOnlyCollection<UserTrack> Tracks => this._objects.AsReadOnly();
   
}
