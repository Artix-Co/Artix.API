namespace Artix.API.Infra.Mongo.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public class MongoCommandRepository<T> : ICommandRepository<T> where T : AggregateRoot
{
    protected readonly MongoCommandContext _commandDbContext;
    protected readonly ILogger<MongoCommandRepository<T>> _logger;

    public MongoCommandRepository(MongoCommandContext commandDbContext, ILogger<MongoCommandRepository<T>> logger)
    {
        this._commandDbContext = commandDbContext;
        _logger = logger;
    }

    public void Insert(T entity)
    {
        this._logger.LogInformation("Inserting entity of type {EntityType} with ID {EntityId}", typeof(T).Name,
            entity.BusinessId);
        this._commandDbContext.Insert(entity);
        this._logger.LogInformation("Entity inserted");
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Inserting entity asynchronously of type {EntityType} with ID {EntityId}",
            typeof(T).Name, entity.BusinessId);
        await this._commandDbContext.InsertAsync(entity, cancellationToken: cancellationToken);
        this._logger.LogInformation("Entity inserted asynchronously");
    }

    public void InsertRange(IEnumerable<T> entities)
    {
        this._logger.LogInformation("Inserting range of entities of type {EntityType}", typeof(T).Name);
        this._commandDbContext.InsertMany(entities);
        this._logger.LogInformation("Range inserted");
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Inserting range asynchronously of type {EntityType}", typeof(T).Name);
        await this._commandDbContext.InsertManyAsync(entities, cancellationToken: cancellationToken);
        this._logger.LogInformation("Range inserted asynchronously");
    }

    public void Update(T entity)
    {
        this._logger.LogInformation("Updating entity of type {EntityType} with ID {EntityId}", typeof(T).Name,
            entity.BusinessId);
        entity.ModifiedAt = DateTime.UtcNow;
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, entity.BusinessId);
        var update = Builders<T>.Update.Set(e => e.ModifiedAt, entity.ModifiedAt);
        this._commandDbContext.Update(filter, update, entity);
        this._logger.LogInformation("Entity updated");
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Updating entity asynchronously of type {EntityType} with ID {EntityId}",
            typeof(T).Name, entity.BusinessId);
        entity.ModifiedAt = DateTime.UtcNow;
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, entity.BusinessId);
        var update = Builders<T>.Update.Set(e => e.ModifiedAt, entity.ModifiedAt);
        await this._commandDbContext.UpdateAsync(filter, update, entity, cancellationToken: cancellationToken);
        this._logger.LogInformation("Entity updated asynchronously");
    }

    public void Delete(Guid businessId)
    {
        this._logger.LogInformation("Soft deleting entity with ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId);
        var update = Builders<T>.Update.Set(e => e.IsDeleted, true);
        this._commandDbContext.Update(filter, update, null); // null for document since it's soft delete
        this._logger.LogInformation("Entity soft deleted");
    }

    public async Task DeleteAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Soft deleting entity asynchronously with ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId);
        var update = Builders<T>.Update.Set(e => e.IsDeleted, true);
        await this._commandDbContext.UpdateAsync(filter, update, null,
            cancellationToken: cancellationToken); // null for document since it's soft delete
        this._logger.LogInformation("Entity soft deleted asynchronously");
    }

    public T? GetById(Guid businessId)
    {
        this._logger.LogInformation("Getting entity by ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId) &
                     Builders<T>.Filter.Eq(e => e.IsDeleted, false);
        var entity = this._commandDbContext.GetCollection<T>(typeof(T).Name.ToLowerInvariant()).Find(filter).FirstOrDefault();
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
        var entity = await this._commandDbContext.GetCollection<T>(typeof(T).Name.ToLowerInvariant()).Find(filter)
            .FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            this._logger.LogWarning("Entity not found with ID {EntityId}", businessId);
        }

        return entity;
    }
}
