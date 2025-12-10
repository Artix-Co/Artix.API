namespace Artix.API.Core.Contract.Features.Tiers.Client.Queries.GetAll;

using Artix.API.Core.Domain.Entities.Object.Enums;

public sealed record AllTierDto(
    int MinScanCount,
    bool? RequiredUpgraded,
    bool? RequiredInCollection,
    int? MinDaysSinceAcquired,
    bool? RequiredSpecial,
    ObjectSaleType? RequiredSaleType,
    string? RequiredMembershipType,
    bool? RequiredActiveStreak,
    bool? RequiredCoOpKey,
    int TierLevel,
    double Multiplier,
    int Priority
);

