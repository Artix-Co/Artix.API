namespace Artix.API.Core.Domain.Entities.Museum;

using Object;

public class MuseumObject
{
    public long MuseumId { get; private set; }
    public virtual Museum Museum { get; private set; }

    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }


    protected MuseumObject()
    {
    }

    private MuseumObject(long objectId, long museumId)
    {
        ObjectId = objectId;
        MuseumId = museumId;
    }

    public static MuseumObject Create(long objectId, long museumId)
    {
        return new MuseumObject(objectId, museumId);
    }

    public void UpdateMuseum(long museumId)
    {
        MuseumId = museumId;
    }
}
