namespace Artix.API.Core.Domain.Entities.Museum;

using Common;
using Events;
using Exceptions;
using Object;

public class Museum : AggregateRoot
{
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private readonly List<MuseumObject> _museumObjects = new();
    public virtual IReadOnlyCollection<MuseumObject> MuseumObjects => _museumObjects.AsReadOnly();


    private readonly List<MuseumImage> _museumImages = new();
    public virtual IReadOnlyCollection<MuseumImage> MuseumImages => this._museumImages.AsReadOnly();


    protected Museum()
    {
    }

    private Museum(string name, string? description, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
        Name = name;
        Description = description;
        IsActive = isActive;
        RaiseDomainEvent(new MuseumCreatedEvent(BusinessId, name, description, isActive));
    }

    public static Museum Create(string name, string? description = null, bool isActive = true)
    {
        return new Museum(name, description, isActive);
    }

    public void UpdateDetails(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw DomainException.InvalidValue(nameof(name));
        Name = name;
        Description = description;

        // RaiseDomainEvent(new MuseumUpdatedEvent(BusinessId, name, description));
    }

    public void Activate()
    {
        if (IsActive)
            throw DomainException.InvalidOperation("Museum is already active.");
        IsActive = true;
    }

    public void Deactivate()
    {
        if (!IsActive)
            throw DomainException.InvalidOperation("Museum is already inactive.");
        IsActive = false;
    }

    public void AddObject(Object obj, string qrCode, bool isSpecial = false, bool isHidden = false)
    {
        if (!IsActive)
            throw DomainException.InvalidOperation("Cannot add objects to an inactive museum.");
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));
        if (_museumObjects.Any(o => o.ObjectId == obj.Id))
            throw DomainException.InvalidOperation("Object already exists in the museum.");

        var museumObject = MuseumObject.Create(obj, this, qrCode, isSpecial, isHidden);
        _museumObjects.Add(museumObject);
    }

    public void RemoveObject(Object obj)
    {
        if (obj == null)
            throw DomainException.InvalidValue(nameof(obj));

        var museumObject = _museumObjects.FirstOrDefault(o => o.ObjectId == obj.Id);
        if (museumObject != null)
            _museumObjects.Remove(museumObject);
    }

    public bool HasObject(long objectId) => MuseumObjects.Any(o => o.ObjectId == objectId);


    public Object? FindObject(Guid objectBusinessId) => MuseumObjects.Select(mo => mo.Object)
        .FirstOrDefault(o => o.BusinessId == objectBusinessId);

   
    
    public void AssignImage(long fileId, string[] allowedMimeTypes)
    {
        var existing = _museumImages.FirstOrDefault(i => i.MuseumId == Id);
        if (existing is not null)
        {
            existing.UpdateFile(fileId, allowedMimeTypes);
            return;
        }

        var museumImage = MuseumImage.Create(Id, fileId);
        _museumImages.Add(museumImage);
    }
}
