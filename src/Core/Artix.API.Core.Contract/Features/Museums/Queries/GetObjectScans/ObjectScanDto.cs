namespace Artix.API.Core.Contract.Features.Museums.Queries.GetObjectScans;

using Domain.Entities.Museum;

public sealed class ObjectScanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? QrCode { get; set; }
    public string? Description { get; set; }
    public int? Version { get; set; }
    public int? Tier { get; set; }
    public bool IsSpecial { get; set; }
    public bool IsHidden { get; set; }
    public Guid MuseumId { get; set; }
    public string MuseumName { get; set; }
    public string? VoiceAssistantAudioBase64 { get; set; }
    public List<Category> Categories { get; set; }
}
