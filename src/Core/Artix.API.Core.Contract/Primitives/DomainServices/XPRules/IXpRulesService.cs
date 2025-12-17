namespace Artix.API.Core.Contract.Primitives.DomainServices.XPRules;

public interface IXpRulesService
{
    Task CalculateXpForFirstScanAsync(long userId, Guid objectId, long? seasonId = null,
        CancellationToken cancellationToken = default);

    Task CalculateXpForRepeatScanAsync(long userId, Guid objectId, long? seasonId = null, bool isGoldenLevel = false,
        CancellationToken cancellationToken = default);
}
