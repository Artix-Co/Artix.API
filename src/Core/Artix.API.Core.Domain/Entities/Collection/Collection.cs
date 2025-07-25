namespace Artix.API.Core.Domain.Entities.Collection;

using Common;
using Exceptions;
using Museum;
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


    private Collection()
    {
    }


    public void AddMuseumObject(MuseumObject museumObject)
    {
        if (_items.Any(i => i.ObjectId == museumObject.Id))
            throw DomainException.BusinessRuleViolation("Object already exists in collection.");

        _items.Add(new CollectionItem { CollectionId = Id, ObjectId = museumObject.Id, Object = museumObject });
    }
}
