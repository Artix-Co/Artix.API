namespace Artix.API.Core.Domain.Entities.Object;

using Exceptions;
using File;

public class ObjectImage
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long FileId { get; private set; }
    public virtual File File { get; private set; }
    
    
    protected ObjectImage() { }

    private ObjectImage(Object obj, File file)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (file == null)
            throw DomainException.InvalidValue(nameof(file));

        this.ObjectId = obj.Id;
        this.FileId = file.Id;
    }
    
    public static ObjectImage Create(Object obj, File file)
    {
        return new ObjectImage(obj, file);
    }
}
