namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetUserRecentMuseumsVisits;

using Contract.Features.Caches.Museums;
using Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetUserRecentMuseumsVisitsQueryHandler : QueryHandlerBase<GetUserRecentMuseumsVisitQuery,
    IEnumerable<UserRecentMuseumsVisitDto>>
{
    private readonly ICacheRepository<RecentMuseumDto> _museumCache;

    public GetUserRecentMuseumsVisitsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ICacheRepository<RecentMuseumDto> museumCache) : base(cache, httpContextAccessor,
        userManager)
    {
        this._museumCache = museumCache;
    }

    public override async Task<Result<IEnumerable<UserRecentMuseumsVisitDto>>> Handle(
        GetUserRecentMuseumsVisitQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        // var recentVisitsCached = await _museumCache.GetAsync(user.Id.ToString());
        // var result = recentVisitsCached.Select(m => new UserRecentMuseumsVisitDto(m.Id, m.ImageUrl, m.Name));
        return Result<IEnumerable<UserRecentMuseumsVisitDto>>.Success([]);
    }
}
