namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;

public sealed record MuseumJournalEntryDto(
    Guid Id,
    Guid MuseumId,
    Guid UserId,
    string? Content,
    DateTime CreatedAt,
    string? Title,
    string? SketchUrl
);
