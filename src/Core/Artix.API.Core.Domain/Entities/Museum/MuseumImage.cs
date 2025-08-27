namespace Artix.API.Core.Domain.Entities.Museum;

using File;
using Exceptions;

public class MuseumImage
{
    public long MuseumId { get; private set; }
    public virtual Museum Museum { get; private set; }

    public long FileId { get; private set; }
    public virtual File File { get; private set; }
    
    
    protected MuseumImage() { }

    private MuseumImage(Museum museum, File file)
    {
        if (museum == null)
            throw DomainException.InvalidValue(nameof(museum));
        if (file == null)
            throw DomainException.InvalidValue(nameof(file));

        this.MuseumId = museum.Id;
        this.FileId = file.Id;
    }
    
    public static MuseumImage Create(Museum museum, File file)
    {
        return new MuseumImage(museum, file);
    }
}
