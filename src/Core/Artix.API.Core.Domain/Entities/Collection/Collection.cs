namespace Artix.API.Core.Domain.Entities.Collection;

using Common;
using Exceptions;
using Museum;
using User;

public class Collection : BaseEntity
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsPublic { get; set; }
    public long UserId { get; set; }

    public virtual AppUser User { get; set; }

    private readonly List<CollectionItem> _items = new();
    public virtual IReadOnlyCollection<CollectionItem> Items => _items.AsReadOnly();


    protected Collection()
    {
    }


    public void AddMuseumObject(MuseumObject museumObject)
    {
        if (_items.Any(i => i.ObjectId == museumObject.Id))
            throw DomainException.BusinessRuleViolation("Object already exists in collection.");

        _items.Add(new CollectionItem { CollectionId = Id, ObjectId = museumObject.Id, Object = museumObject });
    }
}
