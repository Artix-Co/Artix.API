namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetUserRecentObjectsVisits;

using Contract.Features.Caches.Museums;
using Contract.Features.Caches.Objects;
using Contract.Features.Objects.Queries.GetUserRecentObjectsVisits;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class
    GetUserRecentObjectsVisitsQueryHandler : QueryHandlerBase<GetUserRecentObjectsVisitQuery,
    IEnumerable<UserRecentObjectsVisitDto>>
{
    private readonly ICacheService<RecentObjectDto> _museumCache;

    public GetUserRecentObjectsVisitsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ICacheService<RecentObjectDto> museumCache) : base(cache, httpContextAccessor,
        userManager)
    {
        this._museumCache = museumCache;
    }

    public override async Task<Result<IEnumerable<UserRecentObjectsVisitDto>>> Handle(
        GetUserRecentObjectsVisitQuery query,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var recentVisitsCached = await _museumCache.GetRecentAsync(user.Id.ToString());
        var result = recentVisitsCached.Select(m => new UserRecentObjectsVisitDto(m.Id, m.Name));
        return Result<IEnumerable<UserRecentObjectsVisitDto>>.Success(result);
    }
}
