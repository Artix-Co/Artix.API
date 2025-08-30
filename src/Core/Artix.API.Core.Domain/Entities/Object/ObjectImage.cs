namespace Artix.API.Core.Domain.Entities.Object;

using Exceptions;
using File;

public class ObjectImage
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }


    protected ObjectImage()
    {
    }

    private ObjectImage(long objectId, long fileId)
    {
        this.ObjectId = objectId;
        this.FileId = fileId;
    }

    public static ObjectImage Create(long objectId, long fileId)
    {
        return new ObjectImage(objectId, fileId);
    }
}
