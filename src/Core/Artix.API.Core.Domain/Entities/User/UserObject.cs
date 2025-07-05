

namespace Artix.API.Core.Domain.Entities.User;

using _primitives;
using Elastic.CommonSchema;
using Museum;

public class UserObject : BaseEntity
{
    public long UserId { get; private set; }
    public long ObjectId { get; private set; }
    public int ScanCount { get; private set; }
    public DateTime? AcquiredAt { get; private set; }
    public bool IsUpgraded { get; private set; }
    public bool InCollection { get; private set; }

    public virtual MuseumObject Object { get; private set; }
    public virtual AppUser User { get; private set; }

    public UserObject(long userId, long objectId, MuseumObject @object, AppUser user)
    {
        UserId = userId;
        ObjectId = objectId;
        Object = @object ?? throw new ArgumentNullException(nameof(@object));
        User = user ?? throw new ArgumentNullException(nameof(user));
        ScanCount = 0;
        IsUpgraded = false;
        InCollection = false;
        AcquiredAt = null;
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
