

namespace Artix.API.Core.Domain.Entities.User;

using _primitives;
using JournalEntry;

public sealed class UserJournalEntry : BaseEntity
{
  
  
    public DateTime? UnlockedAt { get; private set; }

    public long EntryId { get; private set; }
    public JournalEntry Entry { get; private set; }
    
    public long UserId { get; private set; }
    public AppUser User { get; private set; }

    public void AssignEntry(AppUser user, JournalEntry entry, DateTime? unlockedAt = null)
    {
        User = user ?? throw new ArgumentNullException(nameof(user));
        Entry = entry ?? throw new ArgumentNullException(nameof(entry));
        UserId = user.Id;
        EntryId = entry.Id;
        UnlockedAt = unlockedAt;
        SetModified();
    }
}
