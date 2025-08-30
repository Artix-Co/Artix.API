namespace Artix.API.Core.Domain.Entities.Object;

using File;
using Exceptions;

public class ObjectModel
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }


    protected ObjectModel()
    {
    }

    private ObjectModel(long objectId, long fileId)
    {
        this.ObjectId = objectId;
        this.FileId = fileId;
    }

    public static ObjectModel Create(long objectId, long fileId)
    {
        return new ObjectModel(objectId, fileId);
    }
}
