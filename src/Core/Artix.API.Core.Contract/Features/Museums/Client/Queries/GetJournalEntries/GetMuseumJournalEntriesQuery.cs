namespace Artix.API.Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;

using Primitives.Handlers;
using Primitives.Models;

public sealed record GetMuseumJournalEntriesQuery(long MuseumId) : IQuery<IEnumerable<MuseumJournalEntryDto>>;
