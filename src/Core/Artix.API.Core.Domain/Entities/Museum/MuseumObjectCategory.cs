namespace Artix.API.Core.Domain.Entities.Museum;

public sealed class MuseumObjectCategory
{
    public long MuseumObjectId { get; private set; }
    public MuseumObject MuseumObject { get; private set; }
    public long CategoryId { get; private set; }
    public Category Category { get; private set; }

    private MuseumObjectCategory() { }

    private MuseumObjectCategory(MuseumObject museumObject, Category category)
    {
        if (museumObject == null)
            throw new ArgumentNullException(nameof(museumObject));
        if (category == null)
            throw new ArgumentNullException(nameof(category));

        MuseumObject = museumObject;
        MuseumObjectId = museumObject.Id;
        Category = category;
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
