namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;

public sealed class MuseumJournalEntryDto
{
    public Guid Id { get; set; }
    public Guid MuseumId { get; set; }
    public Guid UserId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Title { get; set; }
    public string? SketchUrl { get; set; }
}
