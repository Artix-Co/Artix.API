namespace Artix.API.Infra.Sql.Repositories.Features.Collections;

using Core.Contract.Features.Collections.Queries;
using Core.Contract.Features.Collections.Queries.GetUserCollection;
using Core.Domain.Entities.Collection;
using Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Primitives;

public sealed class CollectionQueryRepository : QueryRepository<Collection>, ICollectionQueryRepository
{
    private readonly ArtixQueryDbContext _queryDbContext;

    public CollectionQueryRepository(ArtixQueryDbContext queryDbContext) : base(queryDbContext)
    {
        this._queryDbContext = queryDbContext;
    }

    public async Task<UserCollectionDto?> GetUserCollectionAsync(GetUserCollectionQuery dto,
        CancellationToken cancellationToken)
    {
        var query = await this._queryDbContext.Collections
            .FirstOrDefaultAsync(c => c.UserId == dto.UserId && c.Id == dto.CollectionId, cancellationToken);

        if (query is null)
        {
            return null;
        }
        
        var result = new UserCollectionDto
        {
            Id = query.Id,
        };
        return result;
    }
}
