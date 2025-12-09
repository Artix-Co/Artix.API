namespace Artix.API.Core.Contract.Features.Collections;

using Client.Queries.GetUserCollection;
using Domain.Entities.Collection;
using Primitives.Repositories;

public interface ICollectionQueryRepository : IQueryRepository<Collection>
{
    IEnumerable<UserCollectionDto> GetCollectionsByUserId(GetUserCollectionsQuery query);
}
