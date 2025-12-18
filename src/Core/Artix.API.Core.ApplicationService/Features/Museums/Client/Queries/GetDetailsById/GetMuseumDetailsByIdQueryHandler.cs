namespace Artix.API.Core.ApplicationService.Features.Museums.Client.Queries.GetDetailsById;

using Exceptions;
using Primitives;
using Artix.API.Core.Contract.Primitives.Infra.Redis;
using Artix.API.Core.Contract.Primitives.Models;
using CacheKeys;
using Contract.Features.Museums;
using Contract.Features.Museums.Client.Queries.GetDetailByIds;
using Contract.Primitives.Infra.Redis.Caches.Museums;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

// TODO: develop validator for this handler

internal sealed class GetMuseumDetailsByIdQueryHandler
    : QueryHandlerBase<GetMuseumDetailsByIdQuery, MuseumDetailsByIdDto>
{
    private readonly IMuseumQueryRepository _museumQueryRepository;
    private readonly ICacheRepository<List<RecentMuseumDto>> _museumCache;
    private readonly ILogger<GetMuseumDetailsByIdQueryHandler> _logger;

    public GetMuseumDetailsByIdQueryHandler(
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IMuseumQueryRepository museumQueryRepository,
        ICacheRepository<List<RecentMuseumDto>> museumCache,
        ILogger<GetMuseumDetailsByIdQueryHandler> logger)
        : base(httpContextAccessor, userManager)
    {
        _museumQueryRepository = museumQueryRepository;
        _museumCache = museumCache;
        _logger = logger;
    }

    public override async Task<Result<MuseumDetailsByIdDto>> Handle(
        GetMuseumDetailsByIdQuery query,
        CancellationToken cancellationToken)
    {
        // 1. کاربر جاری (owner کش)
        var user = await GetCurrentUserAsync(cancellationToken);

        // 2. کلید کش: فقط وابسته به user (recent = per user)
        var cacheKey = CacheKeys.RecentMuseums(user.BusinessId);

        // 3. گرفتن دیتای اصلی از دیتابیس (source of truth)
        var museumDetailsDto = _museumQueryRepository.GetDetailsById(query);
        if (museumDetailsDto == null)
            throw ApplicationServiceNotFoundException.ForEntity(
                nameof(MuseumDetailsByIdDto),
                query.Id);

        // 4. ساخت آیتم recent از دیتای واقعی
        var recentItem = new RecentMuseumDto(
            museumDetailsDto.Id,
            museumDetailsDto.ImageUrl,
            museumDetailsDto.Name!,
            museumDetailsDto.ObjectCount);

        // 5. خواندن لیست فعلی recentها از کش
        // اگر وجود نداشت، یک لیست جدید می‌سازیم
        var currentList =
            await _museumCache.GetAsync(cacheKey)
            ?? new List<RecentMuseumDto>();

        // 6. اگر قبلاً این موزه دیده شده، حذفش می‌کنیم
        var existingIndex = currentList.FindIndex(x => x.Id == recentItem.Id);
        if (existingIndex >= 0)
            currentList.RemoveAt(existingIndex);

        // 7. اضافه کردن موزه در ابتدای لیست (most recent)
        currentList.Insert(0, recentItem);

        // 8. محدود کردن لیست به 10 آیتم آخر
        if (currentList.Count > 10)
            currentList.RemoveRange(10, currentList.Count - 10);

        // 9. ذخیره‌ی لیست به‌روز شده در کش
        await _museumCache.SetAsync(
            cacheKey,
            currentList,
            ttlSeconds: 1800);

        // 10. لاگ شفاف و قابل دیباگ
        _logger.LogInformation(
            "Recent museums updated. UserId={UserId}, MuseumId={MuseumId}, Count={Count}",
            user.BusinessId,
            recentItem.Id,
            currentList.Count);

        // 11. بازگرداندن پاسخ اصلی
        return Result<MuseumDetailsByIdDto>.Success(museumDetailsDto);
    }
}
