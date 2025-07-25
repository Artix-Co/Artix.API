

namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;
using Museum;

public sealed class UserMuseumKey : BaseEntity
{
    public long UserId { get; private set; }
    public AppUser User { get; private set; }
    
    
    public long MuseumId { get; private set; }
    public Museum Museum { get; private set; }
    
    
    public DateTime? AcquiredAt { get; private set; }

    
    

    public void AssignMuseum(AppUser user, Museum museum, DateTime? acquiredAt = null)
    {
        User = user ??  throw DomainException.InvalidValue(nameof(user));
        Museum = museum ??  throw DomainException.InvalidValue(nameof(museum));
        UserId = user.Id;
        MuseumId = museum.Id;
        AcquiredAt = acquiredAt;
        SetModified();
    }
}
