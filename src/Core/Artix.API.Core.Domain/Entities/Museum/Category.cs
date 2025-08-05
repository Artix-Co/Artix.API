namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    private readonly List<MuseumObjectCategory> _museumObjectCategories = new();
    public virtual IReadOnlyCollection<MuseumObjectCategory> MuseumObjectCategories => _museumObjectCategories.AsReadOnly();

    protected Category()
    {
    }
 
    public static Category Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));

        return new Category { Name = name, Description = description };
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
        

        Name = name;
        Description = description;
    }

    public void AddMuseumObject(MuseumObject museumObject)
    {
        if (museumObject == null)
            throw DomainException.InvalidValue(nameof(museumObject));

        if (_museumObjectCategories.Any(c => c.MuseumObjectId == museumObject.Id))
            return;

        var link = MuseumObjectCategory.Create(museumObject, this);
        _museumObjectCategories.Add(link);
    }

    public void RemoveMuseumObject(MuseumObject museumObject)
    {
        if (museumObject == null)
            throw DomainException.InvalidValue(nameof(museumObject));

        var link = _museumObjectCategories.FirstOrDefault(c => c.MuseumObjectId == museumObject.Id);
        if (link != null)
            _museumObjectCategories.Remove(link);
    }

    public bool HasMuseumObject(long museumObjectId)
    {
        return _museumObjectCategories.Any(c => c.MuseumObjectId == museumObjectId);
    }

    public void ClearMuseumObjects()
    {
        _museumObjectCategories.Clear();
    }
}
