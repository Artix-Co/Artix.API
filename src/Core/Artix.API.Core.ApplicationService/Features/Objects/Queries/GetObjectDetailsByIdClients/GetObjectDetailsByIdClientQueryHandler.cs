namespace Artix.API.Core.ApplicationService.Features.Objects.Queries.GetObjectDetailsByIdClients;

using Contract.Features.Caches.Objects;
using Contract.Features.Objects.Queries;
using Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;
using Contract.Primitives.Infra.Redis;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetObjectDetailsByIdClientQueryHandler : QueryHandlerBase<GetObjectDetailsByIdClientQuery, ObjectDetailsByIdClientDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;
    private readonly ICacheRepository<List<RecentObjectDto>> _objectCache;
    private readonly ILogger<GetObjectDetailsByIdClientQueryHandler> _logger;

    public GetObjectDetailsByIdClientQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectQueryRepository objectQueryRepository,
        ICacheRepository<List<RecentObjectDto>> objectCache,
        ILogger<GetObjectDetailsByIdClientQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        _objectQueryRepository = objectQueryRepository;
        _objectCache = objectCache;
        _logger = logger;
    }

    public override async Task<Result<ObjectDetailsByIdClientDto>> Handle(GetObjectDetailsByIdClientQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-objects:{user.Id}";

        var details = await _objectQueryRepository.GetDetailsByIdAsync(query, cancellationToken);
        if (details == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(ObjectDetailsByIdClientDto), query.Id);

        var recentItem = new RecentObjectDto(
            id: details.Id,
            imageUrl: details.ImageUrl,
            model3DUrl: details.Model3DUrl,
            name: details.Name,
            historicalPeriod: details.HistoricalPeriods);

        var currentList = await _objectCache.GetAsync(cacheKey) ?? new List<RecentObjectDto>();

        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        currentList.Insert(0, recentItem);
        if (currentList.Count > 10)
            currentList = currentList.Take(10).ToList();

        await _objectCache.SetAsync(cacheKey, currentList, ttlSeconds: 1800);

        _logger.LogInformation(
            "Object details retrieved and recent visit updated ObjectId={ObjectId} UserId={UserId}",
            query.Id,
            user.Id);

        return Result<ObjectDetailsByIdClientDto>.Success(details);
    }
}
