namespace Artix.API.Core.Domain.Entities.JournalEntry;

using _primitives;
using User;

public class UserJournalEntry : BaseEntity
{
    public long UserId { get; set; }
    public long EntryId { get; set; }

    public DateTime? UnlockedAt { get; set; }

    public virtual AppUser? User { get; set; }
    public virtual JournalEntry? Entry { get; set; }
}
