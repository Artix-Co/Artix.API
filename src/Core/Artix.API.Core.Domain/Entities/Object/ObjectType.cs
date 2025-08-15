namespace Artix.API.Core.Domain.Entities.Museum;

using Exceptions;

public class ObjectType
{
    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public long TypeId { get; private set; }
    public virtual Type Type { get; private set; }

    protected ObjectType() { }

    private ObjectType(Object obj, Type type)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (type == null)
            throw DomainException.InvalidValue(nameof(type));

        Object = obj;
        ObjectId = obj.Id;
        Type = type;
        TypeId = type.Id;
    }

    public static ObjectType Create(Object obj, Type type)
    {
        return new ObjectType(obj, type);
    }
}
