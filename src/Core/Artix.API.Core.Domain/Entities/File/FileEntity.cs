namespace Artix.API.Core.Domain.Entities.File;

using Common;

public class FileEntity : BaseEntity
{
    public string EntityType { get; set; } // e.g., 'Object', 'MusicTrack'
    public long EntityId { get; set; } // Foreign key to Object.Id or MusicTrack.Id
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public DateTime UploadedAt { get; set; }
    public long? UploadedBy { get; set; }
}
