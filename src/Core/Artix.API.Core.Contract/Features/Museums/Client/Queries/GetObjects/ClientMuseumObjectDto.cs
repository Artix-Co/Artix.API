namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;

public sealed record ClientMuseumObjectDto(
    Guid Id,
    Guid MuseumId,
    string? ImageUrl,
    string Name,
    string? Description,
    DateTime CreatedAt,
    string Slug
);
