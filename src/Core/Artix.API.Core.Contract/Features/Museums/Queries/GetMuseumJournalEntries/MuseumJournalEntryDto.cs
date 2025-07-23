namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;

public sealed class MuseumJournalEntryDto
{
    public long Id { get; set; }
    public long MuseumId { get; set; }
    public long UserId { get; set; }
    public string? Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Title { get; set; }
    public string? SketchUrl { get; set; }
}
