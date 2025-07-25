namespace Artix.API.Core.Domain.Entities.Museum;

using _primitives;

public sealed class Category : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    private readonly List<MuseumObjectCategory> _museumObjectCategories = new();
    public IReadOnlyCollection<MuseumObjectCategory> MuseumObjectCategories => _museumObjectCategories.AsReadOnly();

    private Category() { }

    public static Category Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty or whitespace.");

        return new Category
        {
            Name = name,
            Description = description
        };
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty or whitespace.");

        Name = name;
        Description = description;
    }

    public void AddMuseumObject(MuseumObject museumObject)
    {
        if (museumObject == null)
            throw new ArgumentNullException(nameof(museumObject));

        if (_museumObjectCategories.Any(c => c.MuseumObjectId == museumObject.Id))
            return;

        var link = MuseumObjectCategory.Create(museumObject, this);
        _museumObjectCategories.Add(link);
    }

    public void RemoveMuseumObject(MuseumObject museumObject)
    {
        if (museumObject == null)
            throw new ArgumentNullException(nameof(museumObject));

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
