namespace Artix.API.Core.Domain.Entities.Collection;

using Common;
using Museum;

public class CollectionItem
{
    public long CollectionId { get; set; }
    public virtual Collection Collection { get; set; }

    public long ObjectId { get; set; }
    public virtual MuseumObject Object { get; set; }
}
