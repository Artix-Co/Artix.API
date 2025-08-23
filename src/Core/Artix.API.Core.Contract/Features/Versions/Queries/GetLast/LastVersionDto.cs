namespace Artix.API.Core.Contract.Features.Versions.Queries.GetLast;

public sealed record LastVersionDto(bool IsRequired, bool MinSupported, string? Description, string VersionString);
