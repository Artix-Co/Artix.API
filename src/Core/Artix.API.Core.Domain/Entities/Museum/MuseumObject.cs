namespace Artix.API.Core.Domain.Entities.Museum;

using _primitives;

public class MuseumObject : BaseEntity
{
    public string? Description { get; set; }

    public string? Qrcode { get; set; }

    public int? Version { get; set; }

    public int? Tier { get; set; }


    public long MuseumId { get; set; }
    public string Name { get; set; }
    public string QRCode { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsHidden { get; set; }
}
