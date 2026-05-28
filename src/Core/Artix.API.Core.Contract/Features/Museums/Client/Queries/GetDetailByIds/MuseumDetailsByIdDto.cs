namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;

public sealed record MuseumDetailsByIdDto(
    Guid Id,
    string? Name,
    string? ImageUrl,
    string? Description,
    DateTime CreatedAt,
    bool? IsActive,
    int ObjectCount,
    int JournalEntryCount,
    string Slug);
