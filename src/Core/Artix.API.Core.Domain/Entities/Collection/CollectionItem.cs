namespace Artix.API.Core.Domain.Entities.Collection;

using Common;
using Museum;
using Object;

public class CollectionItem
{
    public long CollectionId { get; private set; }
    public virtual Collection Collection { get; private set; }

    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    protected CollectionItem()
    {
    }

    private CollectionItem(Collection collection, Object obj)
    {
        // TODO: use domain layer exception
        Collection = collection ?? throw new ArgumentNullException(nameof(collection));
        CollectionId = collection.Id;
        Object = obj ?? throw new ArgumentNullException(nameof(obj));
        ObjectId = obj.Id;
    }

    internal static CollectionItem Create(Collection collection, Object obj)
    {
        return new CollectionItem(collection, obj);
    }
}
