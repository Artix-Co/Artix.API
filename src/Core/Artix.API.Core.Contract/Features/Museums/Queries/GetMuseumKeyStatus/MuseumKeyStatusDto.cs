namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;

public sealed record MuseumKeyStatusDto(
    Guid MuseumId,
    bool HasKey,
    DateTime? GrantedAt,
    DateTime? ExpiresAt
);
