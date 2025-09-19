namespace Artix.API.Core.DomainService.Interfaces.TierCalculator;

using Domain.Entities.User;

public interface ITierCalculatorService
{
    Task<(int TierLevel, double Multiplier)> CalculateTierAsync(UserScan userScan,
        CancellationToken cancellationToken = default);
}
