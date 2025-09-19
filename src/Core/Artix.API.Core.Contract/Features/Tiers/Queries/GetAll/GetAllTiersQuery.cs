namespace Artix.API.Core.Contract.Features.Tiers.Queries.GetAll;

using Primitives.Handlers;

public sealed record GetAllTiersQuery() : IQuery<IEnumerable<AllTierDto>>;
