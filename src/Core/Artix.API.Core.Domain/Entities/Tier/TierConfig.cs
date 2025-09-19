namespace Artix.API.Core.Domain.Entities.Tier;

using Common;
using Object.Enums;

public class TierConfig : AggregateRoot
{
    public int MinScanCount { get; private set; }  // Progression: Min scans for hunts/quizzes/exploration (core for QR events, quizzes)
    public bool? RequiredUpgraded { get; private set; }  // Achievement: Upgrades from duplicates/re-scans (prevents frustration in repeats)
    public bool? RequiredInCollection { get; private set; }  // Collection: Must be in user's gallery (ties to personalization/market)
    public int? MinDaysSinceAcquired { get; private set; }  // Loyalty: Time-based for streaks/events (non-daily Strike support)
    public bool? RequiredSpecial { get; private set; }  // Rarity: Special items from events/music collections (exclusivity boost)
    public ObjectSaleType? RequiredSaleType { get; private set; }  // Monetization: Ties to pro packs/subscriptions (welcome packs, seasonal)
    public string? RequiredMembershipType { get; private set; }  // Segmentation: Pro users get higher tiers (adventure path, music exclusives)
    public bool? RequiredActiveStreak { get; private set; }  // Streak: Active Strike flame (event-based, with fuel items for forgiveness)
    public bool? RequiredCoOpKey { get; private set; }  // Social: Co-op unlock (key sharing for limited access, fair progression)
    public int TierLevel { get; private set; }  // Output: Visual level for rankings/collections
    public double Multiplier { get; private set; }  // Reward: XP boost (used in paths, quizzes, market trades)
    public int Priority { get; private set; }  // Matching: Best fit selector

    protected TierConfig() { }  // EF Core

    private TierConfig(
        int minScanCount,
        bool? requiredUpgraded,
        bool? requiredInCollection,
        int? minDaysSinceAcquired,
        bool? requiredSpecial,
        ObjectSaleType? requiredSaleType,
        string? requiredMembershipType,
        bool? requiredActiveStreak,
        bool? requiredCoOpKey,
        int tierLevel,
        double multiplier,
        int priority)
    {
        if (minScanCount < 0) throw new ArgumentException("MinScanCount cannot be negative.");
        if (tierLevel < 1) throw new ArgumentException("TierLevel must start from 1.");
        if (multiplier <= 0) throw new ArgumentException("Multiplier must be positive.");

        this.MinScanCount = minScanCount;
        this.RequiredUpgraded = requiredUpgraded;
        this.RequiredInCollection = requiredInCollection;
        this.MinDaysSinceAcquired = minDaysSinceAcquired;
        this.RequiredSpecial = requiredSpecial;
        this.RequiredSaleType = requiredSaleType;
        this.RequiredMembershipType = requiredMembershipType;
        this.RequiredActiveStreak = requiredActiveStreak;
        this.RequiredCoOpKey = requiredCoOpKey;
        this.TierLevel = tierLevel;
        this.Multiplier = multiplier;
        this.Priority = priority;
    }

    public static TierConfig Create(
        int minScanCount,
        bool? requiredUpgraded = null,
        bool? requiredInCollection = null,
        int? minDaysSinceAcquired = null,
        bool? requiredSpecial = null,
        ObjectSaleType? requiredSaleType = null,
        string? requiredMembershipType = null,
        bool? requiredActiveStreak = null,
        bool? requiredCoOpKey = null,
        int tierLevel = 1,
        double multiplier = 1.0,
        int priority = 1)
    {
        return new TierConfig(minScanCount, requiredUpgraded, requiredInCollection, minDaysSinceAcquired, requiredSpecial,
                              requiredSaleType, requiredMembershipType, requiredActiveStreak, requiredCoOpKey, tierLevel, multiplier, priority);
    }
    
    
}
