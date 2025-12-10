namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetObjectDetailsById;

using Exceptions;
using Primitives;
using Artix.API.Core.Contract.Features.Caches.Objects;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using Contract.Features.Objects;
using Contract.Features.Objects.Client.Queries.GetObjectDetailsById;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class GetObjectDetailsByIdQueryHandler : QueryHandlerBase<GetObjectDetailsByIdQuery, ObjectDetailsByIdDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;
    private readonly ICacheRepository<List<RecentObjectDto>> _objectCache;
    private readonly ILogger<GetObjectDetailsByIdQueryHandler> _logger;

    public GetObjectDetailsByIdQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectQueryRepository objectQueryRepository,
        ICacheRepository<List<RecentObjectDto>> objectCache,
        ILogger<GetObjectDetailsByIdQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        this._objectQueryRepository = objectQueryRepository;
        this._objectCache = objectCache;
        this._logger = logger;
    }

    public override async Task<Result<ObjectDetailsByIdDto>> Handle(GetObjectDetailsByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-objects:{user.Id}";

        var details = await this._objectQueryRepository.GetDetailsByIdAsync(query, cancellationToken);
        if (details == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(ObjectDetailsByIdDto), query.Id);

        var recentItem = new RecentObjectDto(
            id: details.Id,
            imageUrl: details.ImageUrl,
            model3DUrl: details.Model3DUrl,
            name: details.Name,
            historicalPeriod: details.HistoricalPeriods);

        var currentList = await this._objectCache.GetAsync(cacheKey) ?? new List<RecentObjectDto>();

        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        currentList.Insert(0, recentItem);
        if (currentList.Count > 10)
            currentList = currentList.Take(10).ToList();

        await this._objectCache.SetAsync(cacheKey, currentList, ttlSeconds: 1800);

        this._logger.LogInformation(
            "Object details retrieved and recent visit updated ObjectId={ObjectId} UserId={UserId}",
            query.Id,
            user.Id);

        return Result<ObjectDetailsByIdDto>.Success(details);
    }
}
