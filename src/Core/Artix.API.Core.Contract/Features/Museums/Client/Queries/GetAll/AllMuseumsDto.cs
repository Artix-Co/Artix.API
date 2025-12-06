namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseums;

public sealed record AllMuseumsDto(
    Guid Id,
    string? Name,
    int ObjectCount,
    string? ImageUrl,
    string? Description,
    DateTime CreatedAt,
    bool? IsActive);
