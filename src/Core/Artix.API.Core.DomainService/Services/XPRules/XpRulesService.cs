namespace Artix.API.Core.DomainService.Services.XPRules;

using Contract.Features.Objects.Commands;
using Contract.Primitives.Infra.Redis;
using Domain.Entities.Object;
using Domain.Entities.User;
using Interfaces.TierCalculator;
using Interfaces.XPRules;
using Microsoft.AspNetCore.Identity;

public sealed class XpRulesService : IXpRulesService
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITierCalculatorService _tierCalculatorService;
    private readonly ICacheRepository<AppUser> _userCache;
    private readonly ICacheRepository<Object> _objectCache;
    private readonly ILeaderboardService _leaderboardService;
    private readonly IDistributedLockService _lockService;
    private readonly IEventDeduplicationStore _dedupStore;
    private readonly IFeatureToggleService _featureToggle;

    public XpRulesService(
        IObjectCommandRepository objectCommandRepository,
        UserManager<AppUser> userManager,
        ITierCalculatorService tierCalculatorService,
        ICacheRepository<AppUser> userCache,
        ICacheRepository<Object> objectCache,
        ILeaderboardService leaderboardService,
        IDistributedLockService lockService,
        IEventDeduplicationStore dedupStore,
        IFeatureToggleService featureToggle)
    {
        _objectCommandRepository = objectCommandRepository;
        _userManager = userManager;
        _tierCalculatorService = tierCalculatorService;
        _userCache = userCache;
        _objectCache = objectCache;
        _leaderboardService = leaderboardService;
        _lockService = lockService;
        _dedupStore = dedupStore;
        _featureToggle = featureToggle;
    }

    public async Task CalculateXpForFirstScanAsync(
        long userId,
        Guid objectId,
        long? seasonId = null,
        CancellationToken cancellationToken = default)
    {
        var dedupKey = $"xp:firstscan:{userId}:{objectId}";
        if (await _dedupStore.TryMarkProcessedAsync(dedupKey, 86400, cancellationToken))
            return;

        await using var lockHandle = await _lockService.TryAcquireAsync(
            $"user:{userId}:xp",
            TimeSpan.FromSeconds(10),
            cancellationToken);

        if (lockHandle is null)
            throw new InvalidOperationException("Could not acquire lock for XP update.");

        var user = await _userCache.GetOrSetAsync(
            $"user:{userId}",
            () => _userManager.FindByIdAsync(userId.ToString()),
            120,
            cancellationToken);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var objectEntity = await _objectCache.GetOrSetAsync(
            $"object:{objectId}",
            () => _objectCommandRepository.GetByIdAsync(objectId, cancellationToken),
            300,
            cancellationToken);

        if (objectEntity is null)
            throw new InvalidOperationException("Object not found.");

        var userScan = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userScan is null)
            throw new InvalidOperationException("User scan not found.");

        var (tierLevel, multiplier) = await _tierCalculatorService.CalculateTierAsync(userScan, cancellationToken);

        var xpBoostFlag = await _featureToggle.GetFlagAsync("xp_double_event", cancellationToken);
        var effectiveMultiplier = xpBoostFlag == "true" ? multiplier * 2 : multiplier;

        long baseXp = objectEntity.IsSpecial ? 150 : 100;
        baseXp += objectEntity.Tier.GetValueOrDefault() * 10;
        long xpToAdd = (long)(baseXp * effectiveMultiplier);

        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp))
            user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);

        if (seasonId.HasValue)
        {
            var seasonProgress = user.UserSeasonProgresses
                                     .FirstOrDefault(sp => sp.UserId == userId && sp.SeasonId == seasonId.Value)
                                 ?? UserSeasonProgress.Create(userId, seasonId.Value, 0);

            if (!user.UserSeasonProgresses.Contains(seasonProgress))
                user.AddUserSeasonProgress(seasonProgress);

            seasonProgress.AddXp((int)xpToAdd);
        }

        await _userManager.UpdateAsync(user);

        await _leaderboardService.IncrementScoreAsync("global", userId.ToString(), xpToAdd, cancellationToken);
        if (seasonId.HasValue)
            await _leaderboardService.IncrementScoreAsync($"season:{seasonId}", userId.ToString(), xpToAdd,
                cancellationToken);
    }

    public async Task CalculateXpForRepeatScanAsync(
        long userId,
        Guid objectId,
        long? seasonId = null,
        bool isGoldenLevel = false,
        CancellationToken cancellationToken = default)
    {
        var goldenSuffix = isGoldenLevel ? ":golden" : string.Empty;
        var dedupKey = $"xp:repeatscan:{userId}:{objectId}{goldenSuffix}";
        if (await _dedupStore.TryMarkProcessedAsync(dedupKey, 86400, cancellationToken))
            return;

        await using var lockHandle = await _lockService.TryAcquireAsync(
            $"user:{userId}:xp",
            TimeSpan.FromSeconds(10),
            cancellationToken);

        if (lockHandle is null)
            throw new InvalidOperationException("Could not acquire lock for XP update.");

        var user = await _userCache.GetOrSetAsync(
            $"user:{userId}",
            () => _userManager.FindByIdAsync(userId.ToString()),
            120,
            cancellationToken);

        if (user is null)
            throw new InvalidOperationException("User not found.");

        var objectEntity = await _objectCache.GetOrSetAsync(
            $"object:{objectId}",
            () => _objectCommandRepository.GetByIdAsync(objectId, cancellationToken),
            300,
            cancellationToken);

        if (objectEntity is null)
            throw new InvalidOperationException("Object not found.");

        var userObject = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == objectEntity.Id);
        if (userObject is null)
            throw new InvalidOperationException("Object not previously scanned by user.");

        if (!userObject.IsUpgraded)
            userObject.Upgrade();
        else if (isGoldenLevel)
            userObject.RecordScan();

        var (tierLevel, multiplier) = await _tierCalculatorService.CalculateTierAsync(userObject, cancellationToken);

        var xpBoostFlag = await _featureToggle.GetFlagAsync("xp_double_event", cancellationToken);
        var effectiveMultiplier = xpBoostFlag == "true" ? multiplier * 2 : multiplier;

        long baseXp = isGoldenLevel ? 200 : 50;
        baseXp += objectEntity.Tier.GetValueOrDefault() * 5;
        long xpToAdd = (long)(baseXp * effectiveMultiplier);

        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp))
            user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);

        if (seasonId.HasValue)
        {
            var seasonProgress = user.UserSeasonProgresses
                                     .FirstOrDefault(sp => sp.UserId == userId && sp.SeasonId == seasonId.Value)
                                 ?? UserSeasonProgress.Create(userId, seasonId.Value, 0);

            if (!user.UserSeasonProgresses.Contains(seasonProgress))
                user.AddUserSeasonProgress(seasonProgress);

            seasonProgress.AddXp((int)xpToAdd);
        }

        await _userManager.UpdateAsync(user);

        await _leaderboardService.IncrementScoreAsync("global", userId.ToString(), xpToAdd, cancellationToken);
        if (seasonId.HasValue)
            await _leaderboardService.IncrementScoreAsync($"season:{seasonId}", userId.ToString(), xpToAdd,
                cancellationToken);
    }
}
