namespace Artix.API.Infra.Sql.Repositories.Features.Collections;

using Core.Contract.Features.Collections.Queries;
using Core.Contract.Features.Collections.Queries.GetUserCollection;
using Core.Domain.Entities.Collection;
using Data.DbContexts;
using Primitives;

public sealed class CollectionQueryRepository : QueryRepository<Collection>, ICollectionQueryRepository
{
    public CollectionQueryRepository(ArtixQueryDbContext queryDbContext) : base(queryDbContext)
    {
    }

    public IEnumerable<UserCollectionDto> GetCollectionsByUserId(GetUserCollectionsQuery dto)
    {
        return _queryDbContext.Collections
            .Where(c => c.UserId == dto.UserId)
            .Select(c => new UserCollectionDto(c.BusinessId, c.Name, c.Description, c.IsPublic))
            .AsEnumerable();
    }
}
