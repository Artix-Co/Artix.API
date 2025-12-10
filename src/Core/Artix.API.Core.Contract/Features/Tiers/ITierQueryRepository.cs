namespace Artix.API.Core.Contract.Features.Tiers;

using Client.Queries.GetAll;
using Primitives.Repositories;
using Domain.Entities.TierConfig;

public interface ITierQueryRepository : IQueryRepository<TierConfig>
{
    Task<IEnumerable<AllTierDto>> GetAllAsync(GetAllTiersQuery query, CancellationToken cancellationToken = default);
}
