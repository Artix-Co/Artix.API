namespace Artix.API.Core.ApplicationService.Features.Museums.Queries.GetDetailByIds;

using Exceptions;
using Primitives;
using Artix.API.Core.Contract.Features.Caches.Museums;
using Artix.API.Core.Contract.Features.Museums.Commands;
using Artix.API.Core.Contract.Features.Museums.Queries;
using Artix.API.Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Redis.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class GetMuseumByIdQueryHandler : QueryHandlerBase<GetMuseumDetailsByIdQuery, MuseumDetailsByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly ICacheRepository<List<RecentMuseumDto>> _museumCache;
    private readonly ILogger<GetMuseumByIdQueryHandler> _logger;

    public GetMuseumByIdQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumQueryRepository museumQueryRepository,
        ICacheRepository<List<RecentMuseumDto>> museumCache,
        ILogger<GetMuseumByIdQueryHandler> logger) : base(httpContextAccessor, userManager)
    {
        _museumQueryRepository = museumQueryRepository;
        _museumCache = museumCache;
        _logger = logger;
    }

    public override async Task<Result<MuseumDetailsByIdDto>> Handle(GetMuseumDetailsByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await GetCurrentUserAsync(cancellationToken);
        var cacheKey = $"recent-museums:{user.Id}";

        var details = _museumQueryRepository.GetDetailsById(query);
        if (details == null)
            throw ApplicationServiceNotFoundException.ForEntity(nameof(MuseumDetailsByIdDto), query.Id);

        var recentItem = new RecentMuseumDto(details.BusinessId, details.ImageUrl, details.Name!);
        var currentList = await _museumCache.GetAsync(cacheKey) ?? new List<RecentMuseumDto>();

        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        currentList.Insert(0, recentItem);
        if (currentList.Count > 10)
            currentList = currentList.Take(10).ToList();

        await _museumCache.SetAsync(cacheKey, currentList, ttlSeconds: 1800);

        _logger.LogInformation("Museum details retrieved and recent visit updated MuseumId={MuseumId} UserId={UserId}", query.Id, user.Id);

        return Result<MuseumDetailsByIdDto>.Success(details);
    }
}
