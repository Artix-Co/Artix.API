namespace Artix.API.Core.Domain.Entities.JournalEntry;


using Common;
using Museum;
using User;

public class JournalEntry : AggregateRoot
{
    public long ObjectId { get; set; }
    public virtual MuseumObject Object { get; set; }


    public string? Title { get; set; }

    public string? SketchUrl { get; set; }

    public string? Notes { get; set; }


    private readonly List<UserJournalEntry> _userJournalEntries = new();
    public virtual IReadOnlyCollection<UserJournalEntry> UserJournalEntries => _userJournalEntries.AsReadOnly();

    // public void AddUserJournalEntry(UserJournalEntry entry)
    // {
    //     _userJournalEntries.Add(entry);
    //     AddEntity(entry);
    // }
    //
    // public void RemoveUserJournalEntry(UserJournalEntry entry)
    // {
    //     _userJournalEntries.Remove(entry);
    //     RemoveEntity(entry);
    // }
}
