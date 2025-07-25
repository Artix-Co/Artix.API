namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Exceptions;

public sealed class Museum : BaseAggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<MuseumObject> _objects = new();
    public IReadOnlyCollection<MuseumObject> MuseumObjects => _objects.AsReadOnly();

    private Museum()
    {
    }

    private Museum(string name, string description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw DomainException.InvalidValue(nameof(description));

        Name = name;
        Description = description;
        IsActive = isActive;
    }

    public static Museum Create(string name, string description, bool isActive)
    {
        return new Museum(name, description, isActive);
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));

        Name = name;
        Description = description;
    }

    public void Activate()
    {
        if (IsActive)
            throw DomainException.InvalidOperation("Museum is already active");

        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw DomainException.InvalidOperation("Museum is already inactive.");


        IsActive = false;
    }

    public void AddObject(MuseumObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (_objects.Any(o => o.Id == obj.Id))
            throw new InvalidOperationException("Object already exists in the museum.");
        if (obj.MuseumId != Id)
            throw new InvalidOperationException("MuseumObject must belong to this museum.");

        _objects.Add(obj);
        AddEntity(obj);
    }

    public void RemoveObject(MuseumObject obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));

        if (_objects.Remove(obj))
            RemoveEntity(obj);
    }

    public bool HasObject(long museumObjectId)
    {
        return _objects.Any(o => o.Id == museumObjectId);
    }

    public IReadOnlyCollection<MuseumObject> GetVisibleObjects()
    {
        return _objects.Where(o => o.IsVisible()).ToList().AsReadOnly();
    }

    public IReadOnlyCollection<MuseumObject> GetSpecialObjects()
    {
        return _objects.Where(o => o.IsSpecial).ToList().AsReadOnly();
    }
}
