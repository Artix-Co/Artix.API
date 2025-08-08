namespace Artix.API.Core.Contract.Features.Collections.Queries;

using Domain.Entities.Collection;
using GetUserCollection;
using Primitives.Repositories;

public interface ICollectionQueryRepository : IQueryRepository<Collection>
{
    Task<UserCollectionDto?> GetUserCollectionAsync(GetUserCollectionQuery query, CancellationToken cancellationToken);
}
