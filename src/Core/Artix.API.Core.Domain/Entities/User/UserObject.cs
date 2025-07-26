namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Elastic.CommonSchema;
using Exceptions;
using Museum;

public sealed class UserObject : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }


    public long ObjectId { get; private set; }
    public MuseumObject Object { get; private set; }


    public int ScanCount { get; private set; }
    public DateTime? AcquiredAt { get; private set; }
    public bool IsUpgraded { get; private set; }
    public bool InCollection { get; private set; }


    private UserObject()
    {
    }

 
    
    public static UserObject Create(AppUser user, MuseumObject museumObject)
    {
        if (user == null)
            throw DomainException.InvalidValue(nameof(user));
        if (museumObject == null)
            throw DomainException.InvalidValue(nameof(museumObject));
        if (user.Id <= 0)
            throw new ArgumentException("User ID must be positive.", nameof(user));
        if (museumObject.Id <= 0)
            throw new ArgumentException("MuseumObject ID must be positive.", nameof(museumObject));

        return new UserObject
        {
            UserId = user.Id,
            User = user,
            ObjectId = museumObject.Id,
            Object = museumObject,
            ScanCount = 0,
            AcquiredAt = null,
            IsUpgraded = false,
            InCollection = false
        };
    }


    public void RecordScan()
    {
        ScanCount++;
        SetModified();
    }

    public void Upgrade()
    {
        IsUpgraded = true;
        SetModified();
    }

    public void SetInCollection(bool value)
    {
        InCollection = value;
        SetModified();
    }

    public void SetAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
        SetModified();
    }
}
