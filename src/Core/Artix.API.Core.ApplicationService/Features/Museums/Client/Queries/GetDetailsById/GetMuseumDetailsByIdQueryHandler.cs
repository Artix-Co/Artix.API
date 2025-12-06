namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetDetailByIds;

using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.ApplicationService.Primitives;
using Artix.API.Core.Contract.Features.Caches.Museums;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Core.Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class GetMuseumDetailsByIdQueryHandler : QueryHandlerBase<GetMuseumDetailsByIdQuery, MuseumDetailsByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly ICacheRepository<List<RecentMuseumDto>> _museumCache;
    private readonly ILogger<GetMuseumDetailsByIdQueryHandler> _logger;

    public GetMuseumDetailsByIdQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumQueryRepository museumQueryRepository,
        ICacheRepository<List<RecentMuseumDto>> museumCache,
        ILogger<GetMuseumDetailsByIdQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        this._museumQueryRepository = museumQueryRepository;
        this._museumCache = museumCache;
        this._logger = logger;
    }

    public override async Task<Result<MuseumDetailsByIdDto>> Handle(GetMuseumDetailsByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await this.GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-museums:{user.Id}";

        var details = this._museumQueryRepository.GetDetailsById(query);
        if (details == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(MuseumDetailsByIdDto), query.Id);

        var recentItem = new RecentMuseumDto(details.Id, details.ImageUrl, details.Name!);
        var currentList = await this._museumCache.GetAsync(cacheKey) ?? new List<RecentMuseumDto>();

        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        currentList.Insert(0, recentItem);
        if (currentList.Count > 10)
            currentList = currentList.Take(10).ToList();

        await this._museumCache.SetAsync(cacheKey, currentList, ttlSeconds: 1800);

        this._logger.LogInformation("Museum details retrieved and recent visit updated MuseumId={MuseumId} UserId={UserId}", query.Id, user.Id);

        return Result<MuseumDetailsByIdDto>.Success(details);
    }
}
