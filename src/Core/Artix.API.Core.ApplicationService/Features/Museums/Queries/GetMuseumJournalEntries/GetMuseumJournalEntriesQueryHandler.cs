namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetMuseumJournalEntries;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetMuseumJournalEntriesQueryHandler : QueryHandlerBase<GetMuseumJournalEntriesQuery,
    IEnumerable<MuseumJournalEntryDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumJournalEntriesQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache, httpContextAccessor, userManager)
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
