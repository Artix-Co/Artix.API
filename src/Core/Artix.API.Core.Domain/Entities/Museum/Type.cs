namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public class Type : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private readonly List<ObjectType> _objectTypes = new();
    public virtual IReadOnlyCollection<ObjectType> ObjectTypes => _objectTypes.AsReadOnly();

    protected Type() { }

    private Type(string name, string? description)
    {
        ValidateName(name);
        Name = name;
        Description = description;
    }

    public static Type Create(string name, string? description = null)
    {
        return new Type(name, description);
    }

    public void UpdateDetails(string name, string? description = null)
    {
        ValidateName(name);
        Name = name;
        Description = description;
    }

    public void AssignObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        if (_objectTypes.Any(ot => ot.ObjectId == obj.Id))
            return;

        var link = ObjectType.Create(obj, this);
        _objectTypes.Add(link);
    }

    public void RemoveObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        var link = _objectTypes.FirstOrDefault(ot => ot.ObjectId == obj.Id);
        if (link != null)
            _objectTypes.Remove(link);
    }

    public bool HasObject(long objectId) => _objectTypes.Any(ot => ot.ObjectId == objectId);

    public void ClearObjects()
    {
        _objectTypes.Clear();
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
    }
}
