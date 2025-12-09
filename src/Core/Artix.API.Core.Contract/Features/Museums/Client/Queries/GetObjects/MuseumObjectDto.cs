namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;

public sealed record MuseumObjectDto(
    Guid Id,
    Guid MuseumId,
    string? ImageUrl,
    string Name,
    string? Description,
    DateTime CreatedAt
);
