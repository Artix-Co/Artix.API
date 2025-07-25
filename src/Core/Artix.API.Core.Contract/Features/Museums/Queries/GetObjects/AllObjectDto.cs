namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjects;

public sealed class AllObjectDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long MuseumId { get; set; }
    public string QRCode { get; set; } = string.Empty;
    public bool IsSpecial { get; set; }
    public bool IsHidden { get; set; }
    public int? Tier { get; set; }
    public int? Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public IReadOnlyCollection<string> CategoryNames { get; set; } = Array.Empty<string>();
}
