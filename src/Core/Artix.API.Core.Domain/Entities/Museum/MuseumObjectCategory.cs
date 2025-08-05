namespace Artix.API.Core.Domain.Entities.Museum;

using Exceptions;

public class MuseumObjectCategory
{
    public long MuseumObjectId { get; private set; }
    public virtual MuseumObject MuseumObject { get; private set; }
    public long CategoryId { get; private set; }
    public virtual Category Category { get; private set; }

    protected MuseumObjectCategory() { }

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
