namespace Artix.API.Core.Domain.Entities.Object;

using Exceptions;

public class ObjectType
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long TypeId { get; private set; }
    public virtual Category Category { get; private set; }

    protected ObjectType() { }

    private ObjectType(Object obj, Category category)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (category == null)
            throw DomainException.InvalidValue(nameof(category));

        this.Object = obj;
        this.ObjectId = obj.Id;
        this.Category = category;
        this.TypeId = category.Id;
    }

    public static ObjectType Create(Object obj, Category category)
    {
        return new ObjectType(obj, category);
    }
}
