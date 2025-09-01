namespace Artix.API.Core.Domain.Entities.Museum;

using File;
using Exceptions;

public class MuseumImage
{
    public long MuseumId { get; private set; }
    public virtual Museum Museum { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }


    protected MuseumImage()
    {
    }

    private MuseumImage(long museumId, long fileId)
    {
        this.MuseumId = museumId;
        this.FileId = fileId;
    }

    public static MuseumImage Create(long museumId, long fileId)
    {
        return new MuseumImage(museumId, fileId);
    }

    public void UpdateFile(long fileId, string[] allowedMimeTypes)
    {
        FileId = fileId;
    }
}
