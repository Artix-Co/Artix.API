namespace Artix.API.Core.Domain.Entities.Object;

using File;
using Exceptions;

public class Object3DModel
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }
    
    public long FileId { get; private set; }
    public virtual File File { get; private set; }

    
    protected Object3DModel() { }

    private Object3DModel(Object obj, File file)
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
    
    public static Object3DModel Create(Object obj, File file)
    {
        return new Object3DModel(obj, file);
    }
}
