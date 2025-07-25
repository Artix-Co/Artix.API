

namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Elastic.CommonSchema;
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
