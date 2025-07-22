namespace Artix.API.Core.Domain.Entities.Collection;

using _primitives;
using Museum;

public sealed class CollectionItem
{
    public long CollectionId { get; set; }
    public Collection Collection { get; set; }

    public long ObjectId { get; set; }
    public MuseumObject Object { get; set; }
}
