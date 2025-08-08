namespace Artix.API.Core.Domain.Entities.Museum;

using Exceptions;
using File;

public class ObjectFile
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }
    
    public long FileId { get; private set; }
    public virtual File File { get; private set; }

    
    protected ObjectFile() { }

    private ObjectFile(Object obj, File file)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (file == null)
            throw DomainException.InvalidValue(nameof(file));

        this.Object = obj;
        this.ObjectId = obj.Id;
        this.File = file;
        this.FileId = file.Id;
    }
    
    public static ObjectFile Create(Object obj, File file)
    {
        return new ObjectFile(obj, file);
    }
}
