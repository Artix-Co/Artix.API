namespace Artix.API.Infra.Mongo.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public class MongoQueryRepository<T> : IQueryRepository<T> where T : AggregateRoot
{
    protected readonly MongoQueryContext _queryDbContext;
    protected readonly ILogger<MongoQueryRepository<T>> _logger;

    public MongoQueryRepository(MongoQueryContext queryDbContext, ILogger<MongoQueryRepository<T>> logger)
    {
        this._queryDbContext = queryDbContext;
        _logger = logger;
    }

    public T? GetById(Guid businessId)
    {
        this._logger.LogInformation("Getting entity by ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId) &
                     Builders<T>.Filter.Eq(e => e.IsDeleted, false);
        var entity = this._queryDbContext.GetCollection<T>(typeof(T).Name.ToLowerInvariant()).Find(filter).FirstOrDefault();
        if (entity == null)
        {
            this._logger.LogWarning("Entity not found with ID {EntityId}", businessId);
        }

        return entity;
    }

    public async Task<T?> GetByIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Getting entity asynchronously by ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId) &
                     Builders<T>.Filter.Eq(e => e.IsDeleted, false);
        var entity = await this._queryDbContext.GetCollection<T>(typeof(T).Name.ToLowerInvariant()).Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            this._logger.LogWarning("Entity not found with ID {EntityId}", businessId);
        }

        return entity;
    }
}
