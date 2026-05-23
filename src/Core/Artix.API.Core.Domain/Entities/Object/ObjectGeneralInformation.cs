namespace Artix.API.Core.Domain.Entities.Object;

using File;

public class ObjectGeneralInformation
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long FileId { get; private set; }
    public virtual FileEntity FileEntity { get; private set; }

    protected ObjectGeneralInformation()
    {
    }

    private ObjectGeneralInformation(long objectId, long fileId)
    {
        ObjectId = objectId;
        FileId = fileId;
    }

    public static ObjectGeneralInformation Create(long objectId, long fileId)
        => new(objectId, fileId);

    public void UpdateFile(long fileId, string[] allowedMimeTypes)
    {
        FileId = fileId;
    }
}
