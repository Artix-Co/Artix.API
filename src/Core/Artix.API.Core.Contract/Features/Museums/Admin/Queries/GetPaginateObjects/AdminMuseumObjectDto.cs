namespace Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;

public sealed record AdminMuseumObjectDto(
    Guid Id,
    string? ImageUrl,
    string Name,
    string? Description,
    DateTime CreatedAt,
    string Slug
);
