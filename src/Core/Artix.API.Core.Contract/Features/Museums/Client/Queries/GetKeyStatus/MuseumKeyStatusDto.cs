namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus;

public sealed record MuseumKeyStatusDto(
    Guid MuseumId,
    bool HasKey,
    DateTime? GrantedAt,
    DateTime? ExpiresAt
);
