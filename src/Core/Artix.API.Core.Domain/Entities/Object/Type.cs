namespace Artix.API.Core.Domain.Entities.Object;

using Common;
using Exceptions;

public class Type : BaseEntity
{
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private readonly List<ObjectType> _objectTypes = new();
    public virtual IReadOnlyCollection<ObjectType> ObjectTypes => this._objectTypes.AsReadOnly();

    protected Type() { }

    private Type(string name, string? description)
    {
        this.ValidateName(name);
        this.Name = name;
        this.Description = description;
    }

    public static Type Create(string name, string? description = null)
    {
        return new Type(name, description);
    }

    public void UpdateDetails(string name, string? description = null)
    {
        this.ValidateName(name);
        this.Name = name;
        this.Description = description;
    }

    public void AssignObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        if (this._objectTypes.Any(ot => ot.ObjectId == obj.Id))
            return;

        var link = ObjectType.Create(obj, this);
        this._objectTypes.Add(link);
    }

    public void RemoveObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        var link = this._objectTypes.FirstOrDefault(ot => ot.ObjectId == obj.Id);
        if (link != null)
            this._objectTypes.Remove(link);
    }

    public bool HasObject(long objectId) => this._objectTypes.Any(ot => ot.ObjectId == objectId);

    public void ClearObjects()
    {
        this._objectTypes.Clear();
    }

    private void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
    }
}
