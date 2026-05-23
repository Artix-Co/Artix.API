namespace Artix.API.Core.ApplicationService.Features.Objects.Client.Queries.GetObjectDetailsById;

using Exceptions;
using Primitives;
using Artix.API.Core.Contract.Features.Caches.Objects;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using CacheKeys;
using Contract.Features.Objects;
using Contract.Features.Objects.Client.Queries.GetObjectDetailsById;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler
internal sealed class GetObjectDetailsByIdQueryHandler
    : QueryHandlerBase<GetClientObjectDetailsByIdQuery, ClientObjectDetailsByIdDto>
{
    private readonly IObjectQueryRepository _objectQueryRepository;
    private readonly ICacheRepository<List<RecentObjectDto>> _objectCache;
    private readonly ILogger<GetObjectDetailsByIdQueryHandler> _logger;

    public GetObjectDetailsByIdQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IObjectQueryRepository objectQueryRepository,
        ICacheRepository<List<RecentObjectDto>> objectCache,
        ILogger<GetObjectDetailsByIdQueryHandler> logger)
        : base(httpContextAccessor, userManager)
    {
        _objectQueryRepository = objectQueryRepository;
        _objectCache = objectCache;
        _logger = logger;
    }

    public override async Task<Result<ClientObjectDetailsByIdDto>> Handle(
        GetClientObjectDetailsByIdQuery query,
        CancellationToken cancellationToken)
    {
        // 1. کاربر جاری (owner کش)
        var user = await GetCurrentUserAsync(cancellationToken);

        // 2. کلید کش صحیح: recent objects فقط per user
        var cacheKey = CacheKeys.RecentObjects(user.BusinessId);

    

        
        // 3. گرفتن دیتای object از source of truth
        var details = await _objectQueryRepository
            .GetDetailsByIdAsync(query, cancellationToken);

        if (details == null)
            throw ApplicationServiceNotFoundException.ForEntity(
                nameof(ClientObjectDetailsByIdDto),
                query.Id);

        // 4. ساخت DTO برای recent
        var recentItem = new RecentObjectDto(
            id: details.Id,
            imageUrl: details.ImageUrl,
            model3DUrl: details.Model3DUrl,
            generalInformationUrl: details.GeneralInformationUrl,
            specialInformationUrl: details.SpecialInformationUrl,
            name: details.Name,
            historicalPeriod: details.HistoricalPeriods);

        // 5. خواندن لیست فعلی recent objects
        var currentList =
            await _objectCache.GetAsync(cacheKey)
            ?? new List<RecentObjectDto>();

        // 6. حذف object تکراری (اگر قبلاً دیده شده)
        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        // 7. اضافه کردن object به ابتدای لیست
        currentList.Insert(0, recentItem);

        // 8. محدود کردن لیست به 10 آیتم
        if (currentList.Count > 10)
            currentList.RemoveRange(10, currentList.Count - 10);

        
        // 9. ذخیره در کش
        await _objectCache.SetAsync(
            cacheKey,
            currentList,
            ttlSeconds: 1800);

        // 10. لاگ شفاف و دقیق
        _logger.LogInformation(
            "Recent objects updated. UserId={UserId}, ObjectId={ObjectId}, Count={Count}",
            user.BusinessId,
            recentItem.Id,
            currentList.Count);

        // 11. بازگرداندن پاسخ اصلی
        return Result<ClientObjectDetailsByIdDto>.Success(details);
    }
}
