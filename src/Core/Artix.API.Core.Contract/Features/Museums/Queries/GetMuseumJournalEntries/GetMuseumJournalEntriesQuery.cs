namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;

using Primitives.Handlers;

public sealed class GetMuseumJournalEntriesQuery : IQuery<IEnumerable<MuseumJournalEntryDto>>
{
    public long MuseumId { get; init; }
}
