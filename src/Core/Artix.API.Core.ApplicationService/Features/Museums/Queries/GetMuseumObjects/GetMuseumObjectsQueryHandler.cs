namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetMuseumObjects;

using Contract.Features.Museums.Queries;
using Contract.Features.Museums.Queries.GetMuseumObjects;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetMuseumObjectsQueryHandler : QueryHandlerBase<GetMuseumObjectsQuery, IEnumerable<MuseumObjectDto>>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;


    public GetMuseumObjectsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IMuseumQueryRepository museumQueryRepository) : base(cache, httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
    }

    public override async Task<IEnumerable<MuseumObjectDto>> Handle(GetMuseumObjectsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._museumQueryRepository.GetObjectsAsync(query, cancellationToken);
        return result;
    }
}
