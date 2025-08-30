namespace Artix.API.Core.Domain.Entities.Museum;

using File;
using Exceptions;

public class MuseumImage
{
    public long MuseumId { get; private set; }
    public virtual Museum Museum { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }
    
    
    protected MuseumImage() { }

    private MuseumImage(Museum museum, FileEntity fileEntity)
    {
        if (museum == null)
            throw DomainException.InvalidValue(nameof(museum));
        if (fileEntity == null)
            throw DomainException.InvalidValue(nameof(fileEntity));

        this.MuseumId = museum.Id;
        this.FileId = fileEntity.Id;
    }
    
    public static MuseumImage Create(Museum museum, FileEntity fileEntity)
    {
        return new MuseumImage(museum, fileEntity);
    }
}
