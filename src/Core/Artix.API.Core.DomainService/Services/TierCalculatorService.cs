namespace Artix.API.Core.DomainService.Services;

using System.Security.Claims;
using Contract.Features.Tiers;
using Contract.Features.Tiers.Client.Queries.GetAll;
using Contract.Primitives.DomainServices.TierCalculator;
using Domain.Entities.Museum;
using Domain.Entities.Object;
using Domain.Entities.User;
using Microsoft.AspNetCore.Identity;

public sealed class TierCalculatorService : ITierCalculatorService
{
    private readonly ITierQueryRepository _tierQueryRepository;
    private readonly UserManager<AppUser> _userManager;
    // private readonly IEventRepository _eventRepository;  // برای seasonal events

    public TierCalculatorService(
        ITierQueryRepository tierQueryRepository,
        UserManager<AppUser> userManager 

        // IEventRepository eventRepository
    )
    {
        this._tierQueryRepository = tierQueryRepository ?? throw new ArgumentNullException(nameof(tierQueryRepository));
        this._userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

        // _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
    }

    public async Task<(int TierLevel, double Multiplier)> CalculateTierAsync(
        UserScan userScan, CancellationToken cancellationToken = default)
    {
        var strike = userScan.User.UserStrikes.FirstOrDefault();
        var claims = await this._userManager.GetClaimsAsync(userScan.User);
        var museum = userScan.Object.MuseumObjects.FirstOrDefault()!.Museum;
        var museumKey = userScan.User.UserMuseumKeys.FirstOrDefault(umk => umk.Museum.Id == museum.Id);
        // var currentEventId = await _eventRepository.GetActiveEventIdAsync(cancellationToken);

        // لود configs با caching
        var configs = await this._tierQueryRepository.GetAllAsync(new GetAllTiersQuery(), cancellationToken);
        var orderedConfigs = configs.OrderByDescending(c => c.Priority).ToList();

        // محاسبه tier با انعطاف برای کاربر
        var bestMatch = (TierLevel: 1, Multiplier: 1.0); // Default tier
        double bestMatchScore = 0;

        foreach (var tierConfig in orderedConfigs)
        {
            var (isMatch, matchScore) = this.IsMatch(tierConfig, userScan, userScan.Object, museum, museumKey, strike,
                claims, null);
            if (isMatch)
            {
                return (tierConfig.TierLevel, tierConfig.Multiplier);
            }

            // Concession: نزدیک‌ترین tier رو نگه دار
            if (matchScore > bestMatchScore)
            {
                bestMatchScore = matchScore;
                bestMatch = (tierConfig.TierLevel, this.CalculatePartialMultiplier(tierConfig, matchScore));
            }
        }

        return bestMatch;
    }

    private (bool IsMatch, double MatchScore) IsMatch(
        AllTierDto tierConfig, UserScan userScan, Object objectEntity, Museum? museum,
        UserMuseumKey? museumKey, UserStrike? strike, IList<Claim> claims, long? currentEventId)
    {
        int conditionsMet = 0;
        int totalConditions = 0;

        // Progression
        if (tierConfig.MinScanCount > 0)
        {
            totalConditions++;
            if (userScan.ScanCount >= tierConfig.MinScanCount)
                conditionsMet++;
            else
                return (false,
                    this.CalculatePartialScore(conditionsMet, totalConditions, userScan.ScanCount, tierConfig.MinScanCount));
        }

        // Achievements/Collection
        if (tierConfig.RequiredUpgraded.HasValue)
        {
            totalConditions++;
            if (userScan.IsUpgraded == tierConfig.RequiredUpgraded.Value)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        if (tierConfig.RequiredInCollection.HasValue)
        {
            totalConditions++;
            if (userScan.InCollection == tierConfig.RequiredInCollection.Value)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        // Loyalty
        if (tierConfig.MinDaysSinceAcquired.HasValue && userScan.AcquiredAt.HasValue)
        {
            totalConditions++;
            var days = (DateTime.UtcNow - userScan.AcquiredAt.Value).Days;
            if (days >= tierConfig.MinDaysSinceAcquired.Value)
                conditionsMet++;
            else
                return (false,
                    this.CalculatePartialScore(conditionsMet, totalConditions, days, tierConfig.MinDaysSinceAcquired.Value));
        }

        // Streak (با concession برای fuel)
        if (tierConfig.RequiredActiveStreak.HasValue)
        {
            totalConditions++;
            if (strike?.IsActive == tierConfig.RequiredActiveStreak.Value ||
                (strike?.FuelCount > 0 && tierConfig.RequiredActiveStreak.Value))
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        // Rarity/Monetization
        if (tierConfig.RequiredSpecial.HasValue)
        {
            totalConditions++;
            if (objectEntity.IsSpecial == tierConfig.RequiredSpecial.Value)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        if (tierConfig.RequiredSaleType.HasValue)
        {
            totalConditions++;
            if (objectEntity.ObjectSaleType == tierConfig.RequiredSaleType.Value)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        // Segmentation
        if (!string.IsNullOrEmpty(tierConfig.RequiredMembershipType))
        {
            totalConditions++;
            var clientType = claims.FirstOrDefault(c => c.Type == "ClientType")?.Value;
            if (!string.IsNullOrEmpty(clientType) && clientType == tierConfig.RequiredMembershipType)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        // Social
        if (tierConfig.RequiredCoOpKey.HasValue)
        {
            totalConditions++;
            if (museumKey?.IsUnlocked == tierConfig.RequiredCoOpKey.Value)
                conditionsMet++;
            else
                return (false, this.CalculatePartialScore(conditionsMet, totalConditions));
        }

        // Museum application Events(مثل جشن ۱۵۰۰ ساله)
        // if (tierConfig.RequiredEventId.HasValue)
        // {
        //     totalConditions++;
        //     if (tierConfig.RequiredEventId == currentEventId)
        //         conditionsMet++;
        //     else
        //         return (false, CalculatePartialScore(conditionsMet, totalConditions));
        // }

        return (true, this.CalculatePartialScore(conditionsMet, totalConditions));
    }

    private double CalculatePartialScore(int conditionsMet, int totalConditions, double currentValue = 0,
        double requiredValue = 1)
    {
        // محاسبه درصد پیشرفت برای concessions
        double baseScore = totalConditions > 0 ? (double)conditionsMet / totalConditions : 0;
        if (currentValue > 0 && requiredValue > 0)
            baseScore = Math.Min(baseScore, currentValue / requiredValue);
        return baseScore;
    }

    private double CalculatePartialMultiplier(AllTierDto tierConfig, double matchScore)
    {
        // Concession: اگر کامل match نشد، multiplier نسبی بده
        return 1.0 + (tierConfig.Multiplier - 1.0) * Math.Min(matchScore, 0.8); // حداکثر 80% از multiplier اصلی
    }
}
