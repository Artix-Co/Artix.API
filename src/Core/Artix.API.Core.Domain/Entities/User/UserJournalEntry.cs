

namespace Artix.API.Core.Domain.Entities.User;

using Common;
using Exceptions;
using JournalEntry;

public class UserJournalEntry : BaseEntity
{
  
  
    public DateTime? UnlockedAt { get; private set; }

    public long EntryId { get; private set; }
    public virtual JournalEntry Entry { get; private set; }
    
    public long UserId { get; private set; }
    public virtual AppUser User { get; private set; }

    public void AssignEntry(AppUser user, JournalEntry entry, DateTime? unlockedAt = null)
    {
        User = user ??  throw DomainException.InvalidValue(nameof(user));
        Entry = entry ?? throw DomainException.InvalidValue(nameof(entry));
        UserId = user.Id;
        EntryId = entry.Id;
        UnlockedAt = unlockedAt;
        
    }
}
