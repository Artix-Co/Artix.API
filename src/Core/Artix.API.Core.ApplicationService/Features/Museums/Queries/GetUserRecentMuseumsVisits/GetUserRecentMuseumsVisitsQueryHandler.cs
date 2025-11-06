namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetUserRecentMuseumsVisits;

using Contract.Features.Caches.Museums;
using Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Primitives;

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
        _museumCache = museumCache;
        _logger = logger;
    }

    public override async Task<Result<IEnumerable<UserRecentMuseumsVisitDto>>> Handle(
        GetUserRecentMuseumsVisitQuery query,
        CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-museums:{user.Id}";

        var cached = await _museumCache.GetAsync(cacheKey);
        if (cached != null)
        {
            _logger.LogInformation("Cache hit for recent museums UserId={UserId}", user.Id);
            var result = cached.Select(dto => new UserRecentMuseumsVisitDto(
                Id: dto.Id,
                ImageUrl: dto.ImageUrl,
                Name: dto.Name));

            return Result<IEnumerable<UserRecentMuseumsVisitDto>>.Success(result);
        }

        _logger.LogInformation("Cache miss for recent museums UserId={UserId}", user.Id);
        return Result<IEnumerable<UserRecentMuseumsVisitDto>>.Success(Enumerable.Empty<UserRecentMuseumsVisitDto>());
    }
}
