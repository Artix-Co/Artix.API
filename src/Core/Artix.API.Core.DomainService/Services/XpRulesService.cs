namespace Artix.API.Core.DomainService.Services;

using Contract.Features.Objects;
using Contract.Primitives.DomainServices.TierCalculator;
using Contract.Primitives.DomainServices.XPRules;
using Contract.Primitives.Infra.Redis;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

public sealed class XpRulesService : IXpRulesService
{
    private readonly IObjectCommandRepository _objectCommandRepository;
    private readonly UserManager<AppUser> _userManager;
    private readonly ITierCalculatorService _tierCalculatorService;
    private readonly ILeaderboardService _leaderboardService;
    private readonly IDistributedLockService _lockService;
    private readonly IEventDeduplicationStore _dedupStore;
    private readonly IFeatureToggleService _featureToggle;
    private readonly ILogger<XpRulesService> _logger;

    public XpRulesService(
        IObjectCommandRepository objectCommandRepository,
        UserManager<AppUser> userManager,
        ITierCalculatorService tierCalculatorService,
        ILeaderboardService leaderboardService,
        IDistributedLockService lockService,
        IEventDeduplicationStore dedupStore,
        IFeatureToggleService featureToggle,
        ILogger<XpRulesService> logger)
    {
        this._objectCommandRepository = objectCommandRepository;
        this._userManager = userManager;
        this._tierCalculatorService = tierCalculatorService;
        this._leaderboardService = leaderboardService;
        this._lockService = lockService;
        this._dedupStore = dedupStore;
        this._featureToggle = featureToggle;
        this._logger = logger;
    }

    public async Task CalculateXpForFirstScanAsync(
        long userId,
        Guid objectId,
        long? seasonId = null,
        CancellationToken ct = default)
    {
        var dedupKey = $"xp:firstscan:{userId}:{objectId}";
        if (await this._dedupStore.TryMarkProcessedAsync(dedupKey, 86400, ct))
        {
            this._logger.LogInformation("First scan XP skipped (dedup) - User:{UserId} Object:{ObjectId}", userId, objectId);
            return;
        }

        await using var lockHandle =
            await this._lockService.TryAcquireAsync($"user:{userId}:xp", TimeSpan.FromSeconds(10), ct);
        if (lockHandle is null)
        {
            this._logger.LogWarning("Failed to acquire lock for first scan XP - User:{UserId} Object:{ObjectId}", userId,
                objectId);
            throw new InvalidOperationException("Failed to acquire XP lock.");
        }

        var user = await this._userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("User not found.");

        var obj = await this._objectCommandRepository.GetByIdAsync(objectId, ct) ??
                  throw new InvalidOperationException("Object not found.");


        var userScan = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == obj.Id) ??
                       throw new InvalidOperationException("First scan record missing.");

        var (tierLevel, multiplier) = await this._tierCalculatorService.CalculateTierAsync(userScan, ct);
        var isDoubleXp = await this._featureToggle.GetFlagAsync("xp_double_event", ct) == "true";
        var effectiveMultiplier = isDoubleXp ? multiplier * 2 : multiplier;

        long baseXp = obj.IsSpecial ? 150 : 100;
        baseXp += obj.Tier.GetValueOrDefault() * 10;
        long xpToAdd = (long)(baseXp * effectiveMultiplier);

        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp)) user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);

        if (seasonId.HasValue)
        {
            var season = user.UserSeasonProgresses.FirstOrDefault(x => x.SeasonId == seasonId) ??
                         UserSeasonProgress.Create(userId, seasonId.Value, 0);
            if (!user.UserSeasonProgresses.Contains(season)) user.AddUserSeasonProgress(season);
            season.AddXp((int)xpToAdd);
        }

        await this._userManager.UpdateAsync(user);

        await this._leaderboardService.IncrementScoreAsync("global", userId.ToString(), xpToAdd, ct);
        if (seasonId.HasValue)
            await this._leaderboardService.IncrementScoreAsync($"season:{seasonId}", userId.ToString(), xpToAdd, ct);

        this._logger.LogInformation(
            "First scan XP granted - User:{UserId} Object:{ObjectId} XP:+{Xp} (Base:{Base} × Multi:{Multi}{Double}) Tier:{Tier} Special:{Special}",
            userId, objectId, xpToAdd, baseXp, multiplier,
            isDoubleXp ? " ×2" : "", tierLevel, obj.IsSpecial);
    }

    public async Task CalculateXpForRepeatScanAsync(
        long userId,
        Guid objectId,
        long? seasonId = null,
        bool isGoldenLevel = false,
        CancellationToken ct = default)
    {
        var suffix = isGoldenLevel ? ":golden" : string.Empty;
        var dedupKey = $"xp:repeatscan:{userId}:{objectId}{suffix}";
        if (await this._dedupStore.TryMarkProcessedAsync(dedupKey, 86400, ct))
        {
            this._logger.LogInformation("Repeat scan XP skipped (dedup) - User:{UserId} Object:{ObjectId} Golden:{Golden}",
                userId, objectId, isGoldenLevel);
            return;
        }

        await using var lockHandle =
            await this._lockService.TryAcquireAsync($"user:{userId}:xp", TimeSpan.FromSeconds(10), ct);
        if (lockHandle is null)
        {
            this._logger.LogWarning("Failed to acquire lock for repeat scan XP - User:{UserId} Object:{ObjectId}", userId,
                objectId);
            throw new InvalidOperationException("Failed to acquire XP lock.");
        }

        var user = await this._userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("User not found.");
        var obj = await this._objectCommandRepository.GetByIdAsync(objectId, ct) ??
                  throw new InvalidOperationException("Object not found.");


        var userScan = user.UserScans.FirstOrDefault(uo => uo.UserId == userId && uo.ObjectId == obj.Id) ??
                       throw new InvalidOperationException("Object not previously scanned by user.");

        if (!userScan.IsUpgraded)
            userScan.Upgrade();
        else if (isGoldenLevel)
            userScan.RecordScan();

        var (tierLevel, multiplier) = await this._tierCalculatorService.CalculateTierAsync(userScan, ct);
        var isDoubleXp = await this._featureToggle.GetFlagAsync("xp_double_event", ct) == "true";
        var effectiveMultiplier = isDoubleXp ? multiplier * 2 : multiplier;

        long baseXp = isGoldenLevel ? 200 : 50;
        baseXp += obj.Tier.GetValueOrDefault() * 5;
        long xpToAdd = (long)(baseXp * effectiveMultiplier);

        var userXp = user.UserXps.FirstOrDefault() ?? UserXp.Create(userId);
        if (!user.UserXps.Contains(userXp)) user.AddUserXp(userXp);
        userXp.AddXp(xpToAdd);

        if (seasonId.HasValue)
        {
            var season = user.UserSeasonProgresses.FirstOrDefault(x => x.SeasonId == seasonId) ??
                         UserSeasonProgress.Create(userId, seasonId.Value, 0);
            if (!user.UserSeasonProgresses.Contains(season)) user.AddUserSeasonProgress(season);
            season.AddXp((int)xpToAdd);
        }

        await this._userManager.UpdateAsync(user);

        await this._leaderboardService.IncrementScoreAsync("global", userId.ToString(), xpToAdd, ct);
        if (seasonId.HasValue)
            await this._leaderboardService.IncrementScoreAsync($"season:{seasonId}", userId.ToString(), xpToAdd, ct);

        this._logger.LogInformation(
            "Repeat scan XP granted - User:{UserId} Object:{ObjectId} XP:+{Xp} (Base:{Base} × Multi:{Multi}{Double}) Golden:{Golden} Upgraded:{Upgraded} Tier:{Tier}",
            userId, objectId, xpToAdd, baseXp, multiplier,
            isDoubleXp ? " ×2" : "", isGoldenLevel, userScan.IsUpgraded, tierLevel);
    }
}
