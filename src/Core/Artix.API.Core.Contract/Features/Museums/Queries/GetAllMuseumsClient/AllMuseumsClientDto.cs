namespace Artix.API.Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;

public sealed record AllMuseumsClientDto(
    Guid Id,
    string? Name,
    int ObjectCount,
    string? ImageUrl,
    string? Description,
    DateTime CreatedAt,
    bool? IsActive);
