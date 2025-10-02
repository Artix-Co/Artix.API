namespace Artix.API.Infra.Mongo.Primitives;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Artix.API.Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;




public class MongoCommandRepository<T> : ICommandRepository<T> where T : AggregateRoot
{
    private readonly IMongoCollection<T> _collection;
    private readonly ILogger<MongoCommandRepository<T>> _logger;

    public MongoCommandRepository(IMongoDatabase database, ILogger<MongoCommandRepository<T>> logger)
    {
        this._collection = database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        this._logger = logger;
    }

    public void Insert(T entity)
    {
        this._logger.LogInformation("Inserting entity of type {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
        this._collection.InsertOne(entity);
        this._logger.LogInformation("Entity inserted");
    }

    public async Task InsertAsync(T entity, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Inserting entity asynchronously of type {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
        await this._collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        this._logger.LogInformation("Entity inserted asynchronously");
    }

    public void InsertRange(IEnumerable<T> entities)
    {
        this._logger.LogInformation("Inserting range of entities of type {EntityType}", typeof(T).Name);
        this._collection.InsertMany(entities);
        this._logger.LogInformation("Range inserted");
    }

    public async Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Inserting range asynchronously of type {EntityType}", typeof(T).Name);
        await this._collection.InsertManyAsync(entities, cancellationToken: cancellationToken);
        this._logger.LogInformation("Range inserted asynchronously");
    }

    public void Update(T entity)
    {
        this._logger.LogInformation("Updating entity of type {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
        entity.ModifiedAt = DateTime.UtcNow; // مستقیم استفاده می‌کنیم چون T حالا BaseEntity هست
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        this._collection.ReplaceOne(filter, entity);
        this._logger.LogInformation("Entity updated");
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Updating entity asynchronously of type {EntityType} with ID {EntityId}", typeof(T).Name, entity.Id);
        entity.ModifiedAt = DateTime.UtcNow;
        var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
        await this._collection.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);
        this._logger.LogInformation("Entity updated asynchronously");
    }

    public void Delete(Guid businessId)
    {
        this._logger.LogInformation("Soft deleting entity with ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId);
        var update = Builders<T>.Update.Set(e => e.IsDeleted, true);
        this._collection.UpdateOne(filter, update);
        this._logger.LogInformation("Entity soft deleted");
    }

    public async Task DeleteAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Soft deleting entity asynchronously with ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId);
        var update = Builders<T>.Update.Set(e => e.IsDeleted, true);
        await this._collection.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        this._logger.LogInformation("Entity soft deleted asynchronously");
    }

    public T? GetById(Guid businessId)
    {
        this._logger.LogInformation("Getting entity by ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId) & Builders<T>.Filter.Eq(e => e.IsDeleted, false);
        var entity = this._collection.Find(filter).FirstOrDefault();
        if (entity == null)
        {
            this._logger.LogWarning("Entity not found with ID {EntityId}", businessId);
        }
        return entity;
    }

    public async Task<T?> GetByIdAsync(Guid businessId, CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Getting entity asynchronously by ID {EntityId}", businessId);
        var filter = Builders<T>.Filter.Eq(e => e.BusinessId, businessId) & Builders<T>.Filter.Eq(e => e.IsDeleted, false);
        var entity = await this._collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        if (entity == null)
        {
            this._logger.LogWarning("Entity not found with ID {EntityId}", businessId);
        }
        return entity;
    }
}
