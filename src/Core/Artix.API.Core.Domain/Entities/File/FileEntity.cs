namespace Artix.API.Core.Domain.Entities.File;

using Common;
using Museum;
using Object;
using Voice;

public class FileEntity : AggregateRoot
{
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public long FileSize { get; set; }
    public string? MimeType { get; set; }
    public long? UploadedBy { get; set; }


    private readonly List<ObjectModel> _objectModels = new();
    public virtual IReadOnlyCollection<ObjectModel> ObjectModels => _objectModels.AsReadOnly();

    
    private readonly List<ObjectImage> _objectImages = new();
    public virtual IReadOnlyCollection<ObjectImage> ObjectImages => this._objectImages.AsReadOnly();
    
    
    private readonly List<MuseumImage> _museumImages = new();
    public virtual IReadOnlyCollection<MuseumImage> MuseumImages => this._museumImages.AsReadOnly();

    

    private readonly List<VoiceTrackFile> _voiceTrackFiles = new();
    public virtual IReadOnlyCollection<VoiceTrackFile> VoiceTrackFiles => this._voiceTrackFiles.AsReadOnly();


    protected FileEntity()
    {
    }


    private FileEntity(string filename, string filePath, long fileSize, string? mimeType, long? uploadedBy)
    {
        this.FileName = filename;
        this.FilePath = filePath;
        this.FileSize = fileSize;
        this.MimeType = mimeType;
        this.UploadedBy = uploadedBy;
    }


 
    public static FileEntity Create(string filename, string filePath, long fileSize, string? mimeType, long? uploadedBy)
    {
        return new FileEntity(filename, filePath, fileSize, mimeType, uploadedBy);
    }

    

}
