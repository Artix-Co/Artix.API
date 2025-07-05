namespace Artix.API.Core.Domain.Entities.Collection;

using _primitives;
using Museum;

public class CollectionItem : BaseEntity
{
    public long CollectionId { get; set; }

    public long ObjectId { get; set; }

    public virtual Collection? Collection { get; set; }

    public virtual MuseumObject? Object { get; set; }
}
