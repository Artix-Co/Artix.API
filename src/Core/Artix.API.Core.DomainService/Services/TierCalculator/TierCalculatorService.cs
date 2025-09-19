namespace Artix.API.Core.DomainService.Services.TierCalculator;

using System.Security.Claims;
using Contract.Features.Tiers.Queries;
using Contract.Features.Tiers.Queries.GetAll;
using Domain.Entities.User;
using Interfaces.TierCalculator;
using Microsoft.AspNetCore.Identity;

public sealed class TierCalculatorService : ITierCalculatorService
{
    private readonly ITierQueryRepository _tierQueryRepository;
    private readonly UserManager<AppUser> _userManager;

    public TierCalculatorService(ITierQueryRepository tierQueryRepository, UserManager<AppUser> userManager)
    {
        this._tierQueryRepository = tierQueryRepository;
        this._userManager = userManager;
    }

    public async Task<(int TierLevel, double Multiplier)> CalculateTierAsync(UserScan userScan,
        CancellationToken cancellationToken = default)
    {
        var claims = await this._userManager.GetClaimsAsync(userScan.User);
        var museumId = userScan.Object.MuseumObjects.FirstOrDefault()!.Museum.Id;
        var museumKey = userScan.User.UserMuseumKeys.FirstOrDefault(umk => umk.Museum.Id == museumId)!;
        var configs = await _tierQueryRepository.GetAllAsync(new GetAllTiersQuery(), cancellationToken);

        foreach (var config in configs)
        {
            if (IsMatch(config, userScan, museumKey, claims))
            {
                return (config.TierLevel, config.Multiplier);
            }
        }

        return (0, 1.0);
    }

    private bool IsMatch(AllTierDto config, UserScan userScan,
        UserMuseumKey? coOpKey, IList<Claim> claims)
    {
        // Progression
        if (userScan.ScanCount < config.MinScanCount) return false;

        // Achievements/Collection
        if (config.RequiredUpgraded.HasValue && userScan.IsUpgraded != config.RequiredUpgraded.Value) return false;
        if (config.RequiredInCollection.HasValue &&
            userScan.InCollection != config.RequiredInCollection.Value) return false;

        // Loyalty/Streak
        if (config.MinDaysSinceAcquired.HasValue && userScan.AcquiredAt.HasValue)
        {
            var days = (DateTime.UtcNow - userScan.AcquiredAt.Value).Days;
            if (days < config.MinDaysSinceAcquired.Value) return false;
        }

        if (config.RequiredActiveStreak.HasValue && strike?.IsActive != config.RequiredActiveStreak.Value)
            return false;  

        // Rarity/Monetization
        if (config.RequiredSpecial.HasValue && userScan.Object.IsSpecial != config.RequiredSpecial.Value) return false;
        if (config.RequiredSaleType.HasValue && userScan.Object.ObjectSaleType != config.RequiredSaleType.Value)
            return false;

        // Segmentation/Social
        if (!string.IsNullOrEmpty(config.RequiredMembershipType))
        {
            var clientType = claims.FirstOrDefault(c => c.Type == "ClientType")?.Value;
            if (string.IsNullOrEmpty(clientType) || clientType != config.RequiredMembershipType)
                return false;
        }


        if (config.RequiredCoOpKey.HasValue && coOpKey?.IsUnlocked != config.RequiredCoOpKey.Value)
            return false;

        return true;
    }
}
