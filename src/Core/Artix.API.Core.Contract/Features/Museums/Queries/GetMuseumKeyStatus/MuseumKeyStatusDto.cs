namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

public sealed class MuseumKeyStatusDto
{
    public Guid MuseumId { get; set; }
    public bool HasKey { get; set; }
    public DateTime? GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
