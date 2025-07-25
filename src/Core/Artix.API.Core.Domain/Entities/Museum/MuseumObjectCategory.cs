namespace Artix.API.Core.Domain.Entities.Museum;

using Exceptions;

public sealed class MuseumObjectCategory
{
    public long MuseumObjectId { get; private set; }
    public MuseumObject MuseumObject { get; private set; }
    public long CategoryId { get; private set; }
    public Category Category { get; private set; }

    private MuseumObjectCategory() { }

    private MuseumObjectCategory(MuseumObject museumObject, Category category)
    {
        MuseumObject = museumObject ?? throw DomainException.InvalidValue(nameof(museumObject));
        MuseumObjectId = museumObject.Id;
        Category = category ?? throw DomainException.InvalidValue(nameof(category));
        CategoryId = category.Id;
    }

    public static MuseumObjectCategory Create(MuseumObject museumObject, Category category)
    {
        return new MuseumObjectCategory(museumObject, category);
    }

    public bool IsActiveLink()
    {
        return MuseumObject != null && Category != null;
    }
}
