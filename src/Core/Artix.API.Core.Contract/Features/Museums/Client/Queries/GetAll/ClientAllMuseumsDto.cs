namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetAll;

public sealed record ClientAllMuseumsDto(
    Guid Id,
    string? Name,
    int ObjectCount,
    string? ImageUrl,
    string? Description,
    DateTime CreatedAt,
    bool? IsActive,
    string Slug);
