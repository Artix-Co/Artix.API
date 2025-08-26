namespace Artix.API.Core.DomainService.Interfaces.XPRules;

public interface IXpRulesService
{
    Task CalculateXpForFirstScanAsync(long userId, Guid objectId, long? seasonId = null);
    Task CalculateXpForRepeatScanAsync(long userId, Guid objectId, long? seasonId = null, bool isGoldenLevel = false);
}
