namespace Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;

public sealed record MuseumDetailsByIdDto(
    Guid BusinessId,
    string? Name,
    string? ImageUrl,
    string? Description,
    DateTime CreatedAt,
    bool? IsActive,
    int ObjectCount,
    int JournalEntryCount);
