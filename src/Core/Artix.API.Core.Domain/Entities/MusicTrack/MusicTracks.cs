namespace Artix.API.Core.Domain.Entities.MusicTrack;

using _primitives;
using User;

public class MusicTracks : BaseEntity
{
    public string? Title { get; set; }

    public string? Artist { get; set; }

    public string? Url { get; set; }

    public bool? IsFree { get; set; }

    public long? SeasonId { get; set; }


    public virtual ICollection<UserTrack> UserTracks { get; set; } = new List<UserTrack>();
}
