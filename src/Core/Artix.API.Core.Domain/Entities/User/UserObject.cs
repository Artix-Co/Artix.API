namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Elastic.CommonSchema;
using Exceptions;
using Museum;

public class UserObject : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }


    public long ObjectId { get; private set; }
    public virtual MuseumObject Object { get; private set; }


    public int ScanCount { get; private set; }
    public DateTime? AcquiredAt { get; private set; }
    public bool IsUpgraded { get; private set; }
    public bool InCollection { get; private set; }


    protected UserObject()
    {
    }

 
    
    public static UserObject Create(long userId, long objectId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID must be positive.", nameof(userId));
        if (objectId <= 0)
            throw new ArgumentException("MuseumObject ID must be positive.", nameof(objectId));

        return new UserObject
        {
            UserId = userId,
            ObjectId = objectId,
            ScanCount = 0,
            AcquiredAt = null,
            IsUpgraded = false,
            InCollection = false
        };
    }


    public void RecordScan()
    {
        ScanCount++;
        
    }

    public void Upgrade()
    {
        IsUpgraded = true;
        
    }

    public void SetInCollection(bool value)
    {
        InCollection = value;
        
    }

    public void SetAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
        
    }
}
