namespace Artix.API.Core.Domain.Entities.Museum;

using _primitives;

public sealed class MuseumObject : BaseEntity
{
    public string? Description { get; set; }

    public int? Version { get; set; }

    public int? Tier { get; set; }


    public long MuseumId { get; set; }
    public Museum Museum { get; set; }
    
    public string Name { get; set; }
    public string QRCode { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsHidden { get; set; }
}
