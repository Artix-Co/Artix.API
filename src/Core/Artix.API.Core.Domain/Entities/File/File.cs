namespace Artix.API.Core.Domain.Entities.File;

using Common;
using Museum;
using Voice;

public class File : AggregateRoot
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


    protected File()
    {
    }


    private File(string filename, string filePath, long fileSize, string? mimeType, long? uploadedBy)
    {
        this.FileName = filename;
        this.FilePath = filePath;
        this.FileSize = fileSize;
        this.MimeType = mimeType;
        this.UploadedBy = uploadedBy;
    }


 
    public static File Create(string filename, string filePath, long fileSize, string? mimeType, long? uploadedBy)
    {
        return new File(filename, filePath, fileSize, mimeType, uploadedBy);
    }

    

}
