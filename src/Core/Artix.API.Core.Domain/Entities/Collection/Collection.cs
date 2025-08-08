namespace Artix.API.Core.Domain.Entities.Collection;

using Common;
using Exceptions;
using Museum;
using User;

public class Collection : BaseEntity
{
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsPublic { get; private set; }
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }

    private readonly List<CollectionItem> _items = new();
    public virtual IReadOnlyCollection<CollectionItem> Items => _items.AsReadOnly();

    protected Collection()
    {
    }

    private Collection(string? name, string? description, bool isPublic, long userId, AppUser user)
    {
        Name = name;
        Description = description;
        IsPublic = isPublic;
        UserId = userId;
        User = user ?? throw new ArgumentNullException(nameof(user));
    }

    public static Collection Create(string? name, string? description, long userId, AppUser user)
    {
        return new Collection(name, description, true, userId, user);
    }

    public void AddItem(CollectionItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (_items.Any(ci => ci.ObjectId == item.ObjectId))
            return;

        _items.Add(item);
    }

    public void RemoveItem(CollectionItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        _items.Remove(item);
    }
}
