namespace Artix.API.Infra.Sql.Repositories.Features.Tiers;

using Core.Contract.Features.Tiers.Queries;
using Core.Contract.Features.Tiers.Queries.GetAll;
using Core.Domain.Entities.Tier;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Primitives;

public sealed class TierQueryRepository : QueryRepository<TierConfig>, ITierQueryRepository
{
    public TierQueryRepository(ArtixQueryDbContext queryDbContext) : base(queryDbContext)
    {
    }

    public async Task<IEnumerable<AllTierDto>> GetAllAsync(
        GetAllTiersQuery dto,
        CancellationToken cancellationToken = default)
    {
        return await this._queryDbContext.TierConfigs
            .OrderByDescending(c => c.Priority)
            .Select(tc => new AllTierDto(
                tc.Id,
                tc.BusinessId,
                tc.MinScanCount,
                tc.RequiredUpgraded,
                tc.RequiredInCollection,
                tc.MinDaysSinceAcquired,
                tc.RequiredSpecial,
                tc.RequiredSaleType,
                tc.RequiredMembershipType,
                tc.RequiredActiveStreak,
                tc.RequiredCoOpKey,
                tc.TierLevel,
                tc.Multiplier,
                tc.Priority
            ))
            .ToListAsync(cancellationToken);
    }
}
