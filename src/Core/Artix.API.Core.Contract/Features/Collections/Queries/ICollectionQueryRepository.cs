namespace Artix.API.Core.Contract.Features.Collections.Queries;

using Domain.Entities.Collection;
using GetUserCollection;
using Primitives.Models;
using Primitives.Repositories;

public interface ICollectionQueryRepository : IQueryRepository<Collection>
{
    IEnumerable<UserCollectionDto> GetCollectionsByUserId(GetUserCollectionsQuery query);
}
