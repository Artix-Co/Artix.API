namespace Artix.API.Core.Domain.Entities.Collection;

using _primitives;
using User;

public sealed class Collection : BaseAggregateRoot
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }
    public long UserId { get; set; }

    public AppUser User { get; set; }

    private readonly List<CollectionItem> _items = new();
    public IReadOnlyCollection<CollectionItem> Items => _items.AsReadOnly();

}
