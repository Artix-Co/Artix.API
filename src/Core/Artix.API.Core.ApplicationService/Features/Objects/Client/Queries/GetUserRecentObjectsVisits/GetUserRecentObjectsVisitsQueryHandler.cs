namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetUserRecentObjectsVisits;

using Primitives;
using Artix.API.Core.Contract.Features.Caches.Objects;
using Artix.API.Core.Contract.Features.Objects.Queries.GetUserRecentObjectsVisits;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class GetUserRecentObjectsVisitsQueryHandler : QueryHandlerBase<GetUserRecentObjectsVisitQuery, IEnumerable<UserRecentObjectsVisitDto>>
{
    private readonly ICacheRepository<List<RecentObjectDto>> _objectCache;
    private readonly ILogger<GetUserRecentObjectsVisitsQueryHandler> _logger;

    public GetUserRecentObjectsVisitsQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        ICacheRepository<List<RecentObjectDto>> objectCache,
        ILogger<GetUserRecentObjectsVisitsQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        this._objectCache = objectCache;
        this._logger = logger;
    }

    public override async Task<Result<IEnumerable<UserRecentObjectsVisitDto>>> Handle(
        GetUserRecentObjectsVisitQuery query,
        CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-objects:{user.Id}";

        var cached = await this._objectCache.GetAsync(cacheKey);
        if (cached != null)
        {
            this._logger.LogInformation("Cache hit for recent objects UserId={UserId}", user.Id);
            var result = cached.Select(dto => new UserRecentObjectsVisitDto(
                Id: dto.Id,
                ImageUrl: dto.ImageUrl,
                Model3DUrl: dto.Model3DUrl,
                Name: dto.Name,
                HistoricalPeriod: dto.HistoricalPeriod));

            return Result<IEnumerable<UserRecentObjectsVisitDto>>.Success(result);
        }

        this._logger.LogInformation("Cache miss for recent objects UserId={UserId}", user.Id);
        return Result<IEnumerable<UserRecentObjectsVisitDto>>.Success(Enumerable.Empty<UserRecentObjectsVisitDto>());
    }
}
