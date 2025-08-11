namespace Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;

public sealed class MuseumByIdDto
{
    public long Id { get; set; }
    public Guid BusinessId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public bool? IsActive { get; set; }
    public int ObjectCount { get; set; }
    public int JournalEntryCount { get; set; }
}
