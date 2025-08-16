namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Object;
using Object.Events;

public class UserObject : BaseEntity
{
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }

    public long ObjectId { get; private set; }
    public virtual Object Object { get; private set; }

    public int ScanCount { get; private set; }
    public DateTime? AcquiredAt { get; private set; }
    public bool IsUpgraded { get; private set; }
    public bool InCollection { get; private set; }

    protected UserObject()
    {
    }

    private UserObject(long userId, long objectId)
    {
        UserId = userId;
        ObjectId = objectId;
        ScanCount = 0;
        InCollection = false;
        IsUpgraded = false;
        AcquiredAt = null;
    }

    public static UserObject Create(long userId, long objectId)
    {
        return new UserObject(userId, objectId);
    }

    public void AssignToUser(DateTime acquiredAt)
    {
        if (IsUpgraded)
            return;
        RecordScan();
        SetInCollection(true);
        SetAcquiredAt(acquiredAt);
    }

    public void RecordScan()
    {
        ScanCount++;
    }

    public void SetInCollection(bool inCollection)
    {
        InCollection = inCollection;
    }

    public void SetAcquiredAt(DateTime acquiredAt)
    {
        AcquiredAt = acquiredAt;
    }

    public void Upgrade()
    {
        IsUpgraded = true;
        RecordScan();
    }
}
