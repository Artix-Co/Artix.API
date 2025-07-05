namespace Artix.API.Core.Domain.Entities.Collection;

using _primitives;
using User;

public class Collection : BaseAggregateRoot
{
    public long UserId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public virtual AppUser? User { get; set; }

    private readonly List<CollectionItem> _items = new();
    public IReadOnlyCollection<CollectionItem> Items => _items.AsReadOnly();

    public void AddItem(CollectionItem item)
    {
        _items.Add(item);
        AddEntity(item);
    }

    public void RemoveItem(CollectionItem item)
    {
        _items.Remove(item);
        RemoveEntity(item);
    }

    public void ClearItems()
    {
        _items.Clear();
        SetModified();
    }
}
