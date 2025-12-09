namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetJournalEntries;

using Contract.Features.Museums;
using Contract.Features.Museums.Client.Queries;
using Contract.Features.Museums.Client.Queries.GetJournalEntries;
using Contract.Features.Museums.Queries;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetMuseumJournalEntriesQueryHandler : QueryHandlerBase<GetMuseumJournalEntriesQuery,
    IEnumerable<MuseumJournalEntryDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumJournalEntriesQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<Result<IEnumerable<MuseumJournalEntryDto>>> Handle(GetMuseumJournalEntriesQuery query,
        CancellationToken cancellationToken)
    {
        var result = this._museumQueryRepository.GetJournalEntries(query);
        return Result<IEnumerable<MuseumJournalEntryDto>>.Success(result);
    }
}
