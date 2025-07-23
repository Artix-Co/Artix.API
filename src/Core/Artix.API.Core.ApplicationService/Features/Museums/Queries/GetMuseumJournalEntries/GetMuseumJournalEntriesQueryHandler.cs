namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetMuseumJournalEntries;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetMuseumJournalEntriesQueryHandler : QueryHandlerBase<GetMuseumJournalEntriesQuery,
    IEnumerable<MuseumJournalEntryDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;

    public GetMuseumJournalEntriesQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        IMuseumQueryRepository museumQueryRepository) : base(
        cache, httpContextAccessor)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<IEnumerable<MuseumJournalEntryDto>> Handle(GetMuseumJournalEntriesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetJournalEntriesAsync(query, cancellationToken);
        return result;
    }
}
