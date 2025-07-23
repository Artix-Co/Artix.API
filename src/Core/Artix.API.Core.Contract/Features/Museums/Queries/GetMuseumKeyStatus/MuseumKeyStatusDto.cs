namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

public sealed class MuseumKeyStatusDto
{
    public long MuseumId { get; set; }
    public long UserId { get; set; }
    public bool HasKey { get; set; }
    public DateTime? GrantedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
