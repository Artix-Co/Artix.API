namespace Artix.API.Core.Contract.Features.Museums.Client.Queries.GetJournalEntries;

using Primitives.Handlers;

public sealed record GetMuseumJournalEntriesQuery(long MuseumId) : IQuery<IEnumerable<MuseumJournalEntryDto>>;
