namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumObjects;

public sealed record MuseumObjectDto(
    Guid Id,
    Guid MuseumId,
    string Name,
    string? Description,
    DateTime CreatedAt
);
