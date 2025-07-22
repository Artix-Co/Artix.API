namespace Artix.API.Core.Domain.Entities.Museum;

using _primitives;

public sealed class Museum : BaseAggregateRoot
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool? IsActive { get; set; }


    private readonly List<MuseumObject> _objects = new();
    public IReadOnlyCollection<MuseumObject> Objects => _objects.AsReadOnly();

    public void AddObject(MuseumObject obj)
    {
        _objects.Add(obj);
        AddEntity(obj);
    }

    public void RemoveObject(MuseumObject obj)
    {
        _objects.Remove(obj);
        RemoveEntity(obj);
    }
}
