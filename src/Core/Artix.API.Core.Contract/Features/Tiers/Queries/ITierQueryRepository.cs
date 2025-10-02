namespace Artix.API.Core.Contract.Features.Tiers.Queries;

using Domain.Entities.TierConfig;
using GetAll;
using Primitives.Repositories;

public interface ITierQueryRepository : IQueryRepository<TierConfig>
{
    Task<IEnumerable<AllTierDto>> GetAllAsync(GetAllTiersQuery query, CancellationToken cancellationToken = default);
}
