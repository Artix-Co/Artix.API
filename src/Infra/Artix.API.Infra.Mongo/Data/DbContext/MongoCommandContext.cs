namespace Artix.API.Infra.Mongo.Data.DbContext;

using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Domain.Entities.Common;
using Interceptors;

 

public sealed class MongoCommandContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoTimestampInterceptor _interceptor;

    public MongoCommandContext(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
        _interceptor = new MongoTimestampInterceptor();
    }

    // Get a collection by name
    public IMongoCollection<T> GetCollection<T>(string name) where T : AggregateRoot
    {
        return _database.GetCollection<T>(name ?? throw new ArgumentNullException(nameof(name)));
    }

    // Insert a document (async)
    public async Task InsertAsync<T>(T document, IClientSessionHandle? session = null, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        if (document is not BaseEntity)
        {
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
        }

        // Apply interceptor before insert
        await _interceptor.BeforeInsertAsync(document, cancellationToken);

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        await collection.InsertOneAsync(session, document, null, cancellationToken);
    }

    // Insert a document (sync)
    public void Insert<T>(T document, IClientSessionHandle? session = null) where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        if (document is not BaseEntity)
        {
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
        }

        // Apply interceptor before insert
        _interceptor.BeforeInsert(document);

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        collection.InsertOne(session, document);
    }

    // Insert multiple documents (async)
    public async Task InsertManyAsync<T>(IEnumerable<T> documents, IClientSessionHandle? session = null, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (documents == null) throw new ArgumentNullException(nameof(documents));

        foreach (var document in documents)
        {
            if (document is not BaseEntity)
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
            }
            await _interceptor.BeforeInsertAsync(document, cancellationToken);
        }

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        await collection.InsertManyAsync(session, documents, null, cancellationToken);
    }

    // Insert multiple documents (sync)
    public void InsertMany<T>(IEnumerable<T> documents, IClientSessionHandle? session = null) where T : AggregateRoot
    {
        if (documents == null) throw new ArgumentNullException(nameof(documents));

        foreach (var document in documents)
        {
            if (document is not BaseEntity)
            {
                throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
            }
            _interceptor.BeforeInsert(document);
        }

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        collection.InsertMany(session, documents);
    }

    // Update a document (async)
    public async Task UpdateAsync<T>(FilterDefinition<T> filter, UpdateDefinition<T> update, T document, IClientSessionHandle? session = null, CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        if (update == null) throw new ArgumentNullException(nameof(update));

        if (document is not BaseEntity baseEntity)
        {
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
        }

        // Apply interceptor before update
        await _interceptor.BeforeUpdateAsync(document, cancellationToken);

        // Combine ModifiedAt update with the provided update
        var modifiedAtUpdate = Builders<T>.Update.Set(d => ((BaseEntity)d).ModifiedAt, baseEntity.ModifiedAt);
        var combinedUpdate = Builders<T>.Update.Combine(new[] { modifiedAtUpdate, update });

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        await collection.UpdateOneAsync(session, filter, combinedUpdate, null, cancellationToken);
    }

    // Update a document (sync)
    public void Update<T>(FilterDefinition<T> filter, UpdateDefinition<T> update, T document, IClientSessionHandle? session = null) where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));
        if (filter == null) throw new ArgumentNullException(nameof(filter));
        if (update == null) throw new ArgumentNullException(nameof(update));

        if (document is not BaseEntity baseEntity)
        {
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");
        }

        // Apply interceptor before update
        _interceptor.BeforeUpdate(document);

        // Combine ModifiedAt update with the provided update
        var modifiedAtUpdate = Builders<T>.Update.Set(d => ((BaseEntity)d).ModifiedAt, baseEntity.ModifiedAt);
        var combinedUpdate = Builders<T>.Update.Combine(new[] { modifiedAtUpdate, update });

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        collection.UpdateOne(session, filter, combinedUpdate);
    }

    // Soft delete a document (async)
    public async Task DeleteAsync<T>(T document, IClientSessionHandle? session = null, CancellationToken cancellationToken = default) 
        where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        if (document is not BaseEntity baseEntity)
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");

        // تغییر وضعیت با استفاده از متد entity
        baseEntity.MarkAsDeleted();

        // interceptor
        await _interceptor.BeforeUpdateAsync(document, cancellationToken);

        var filter = Builders<T>.Filter.Eq("_id", document.BusinessId);
        var update = Builders<T>.Update
            .Set(d => d.IsDeleted, baseEntity.IsDeleted)
            .Set(d => d.ModifiedAt, baseEntity.ModifiedAt);

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        await collection.UpdateOneAsync(session, filter, update, null, cancellationToken);
    }

// Soft delete a document (sync)
    public void Delete<T>(T document, IClientSessionHandle? session = null) 
        where T : AggregateRoot
    {
        if (document == null) throw new ArgumentNullException(nameof(document));

        if (document is not BaseEntity baseEntity)
            throw new InvalidOperationException($"Entity of type {typeof(T).Name} must inherit from BaseEntity");

        // تغییر وضعیت با استفاده از متد entity
        baseEntity.MarkAsDeleted();

        // interceptor
        _interceptor.BeforeUpdate(document);

        var filter = Builders<T>.Filter.Eq("_id", document.BusinessId);
        var update = Builders<T>.Update
            .Set(d => d.IsDeleted, baseEntity.IsDeleted)
            .Set(d => d.ModifiedAt, baseEntity.ModifiedAt);

        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        collection.UpdateOne(session, filter, update);
    }

}




