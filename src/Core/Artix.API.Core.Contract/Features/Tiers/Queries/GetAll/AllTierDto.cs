namespace Artix.API.Core.Contract.Features.Tiers.Queries.GetAll;

using Domain.Entities.Object.Enums;

public sealed record AllTierDto(
    long Id,
    Guid BusinessId,
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

