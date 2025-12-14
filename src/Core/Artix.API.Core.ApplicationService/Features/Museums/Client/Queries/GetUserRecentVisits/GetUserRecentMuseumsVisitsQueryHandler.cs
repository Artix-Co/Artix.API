namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetUserRecentVisits;

using Primitives;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Museums.Client.Queries.GetUserRecentVisits;
using Contract.Primitives.Infra.Redis.Caches.Museums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class
    GetUserRecentMuseumsVisitsQueryHandler : QueryHandlerBase<GetUserRecentMuseumsVisitQuery,
    IEnumerable<UserRecentMuseumsVisitDto>>
{
    private readonly ICacheRepository<List<RecentMuseumDto>> _museumCache;
    private readonly ILogger<GetUserRecentMuseumsVisitsQueryHandler> _logger;

    public GetUserRecentMuseumsVisitsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ICacheRepository<List<RecentMuseumDto>> museumCache,
        ILogger<GetUserRecentMuseumsVisitsQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        this._museumCache = museumCache;
        this._logger = logger;
    }

    public override async Task<Result<IEnumerable<UserRecentMuseumsVisitDto>>> Handle(
        GetUserRecentMuseumsVisitQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-museums:{user.BusinessId}";

        var cached = await this._museumCache.GetAsync(cacheKey);
        if (cached != null)
        {
            this._logger.LogInformation("Cache hit for recent museums UserId={UserId}", user.BusinessId);
            var result = cached.Select(dto => new UserRecentMuseumsVisitDto(
                Id: dto.Id,
                ImageUrl: dto.ImageUrl,
                Name: dto.Name,
                ObjectCount: dto.ObjectCount));

            return Result<IEnumerable<UserRecentMuseumsVisitDto>>.Success(result);
        }

        this._logger.LogInformation("Cache miss for recent museums UserId={UserId}", user.BusinessId);
        return Result<IEnumerable<UserRecentMuseumsVisitDto>>.Success([]);
    }
}
