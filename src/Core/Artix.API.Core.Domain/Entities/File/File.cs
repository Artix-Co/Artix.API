namespace Artix.API.Core.Domain.Entities.File;

using Common;
using Museum;
using Voice;

public class File : BaseEntity
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public long? UploadedBy { get; set; }
    
    
        
    private readonly List<ObjectFile> _objectFiles = new();
    public virtual IReadOnlyCollection<ObjectFile> ObjectFiles => _objectFiles.AsReadOnly();
    
    
    private readonly List<VoiceTrackFile> _voiceTrackFiles = new();
    public virtual IReadOnlyCollection<VoiceTrackFile> VoiceTrackFiles => this._voiceTrackFiles.AsReadOnly();

}
