namespace Artix.API.Core.Contract.Features.Tiers.Queries;

using GetAll;

public interface ITierQueryRepository
{
    Task<IEnumerable<AllTierDto>> GetAllAsync(GetAllTiersQuery query, CancellationToken cancellationToken = default);
}
