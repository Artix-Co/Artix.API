namespace Artix.API.Infra.Mongo.Data.Interceptors;

using Core.Domain.Entities.Common;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class MongoTimestampInterceptor : IMongoInterceptor
{
    private readonly IMongoDatabase _database;

    public MongoTimestampInterceptor(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public void BeforeInsert<T>(T document) where T : AggregateRoot
    {
        if (document == null) return;

        // Assign auto-incrementing ID
        document.Id = GetNextSequenceValue(typeof(T).Name);
        document.CreatedAt = DateTime.UtcNow;
        document.ModifiedAt = null; // Reset ModifiedAt for new documents
    }

    public void BeforeUpdate<T>(T document) where T : AggregateRoot
    {
        if (document == null) return;

        document.ModifiedAt = DateTime.UtcNow;
    }

    public async Task BeforeInsertAsync<T>(T document, CancellationToken cancellationToken) where T : AggregateRoot
    {
        if (document == null) return;

        // Assign auto-incrementing ID asynchronously
        document.Id = await GetNextSequenceValueAsync(typeof(T).Name, cancellationToken);
        document.CreatedAt = DateTime.UtcNow;
        document.ModifiedAt = null; // Reset ModifiedAt for new documents
    }

    public async Task BeforeUpdateAsync<T>(T document, CancellationToken cancellationToken) where T : AggregateRoot
    {
        if (document == null) return;

        document.ModifiedAt = DateTime.UtcNow;
    }

    private async Task<long> GetNextSequenceValueAsync(string collectionName, CancellationToken cancellationToken)
    {
        var countersCollection = _database.GetCollection<Counter>("counters");

        var filter = Builders<Counter>.Filter.Eq(c => c.Id, collectionName);
        var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
        var options = new FindOneAndUpdateOptions<Counter>
        {
            ReturnDocument = ReturnDocument.After, IsUpsert = true // Create the document if it doesn't exist
        };

        try
        {
            var counter = await countersCollection
                .FindOneAndUpdateAsync(filter, update, options, cancellationToken);
            return counter.SequenceValue;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate sequence value for {collectionName}: {ex.Message}",
                ex);
        }
    }

    private long GetNextSequenceValue(string collectionName)
    {
        var countersCollection = _database.GetCollection<Counter>("counters");

        var filter = Builders<Counter>.Filter.Eq(c => c.Id, collectionName);
        var update = Builders<Counter>.Update.Inc(c => c.SequenceValue, 1);
        var options = new FindOneAndUpdateOptions<Counter> { ReturnDocument = ReturnDocument.After, IsUpsert = true };

        try
        {
            var counter = countersCollection.FindOneAndUpdate(filter, update, options);
            return counter.SequenceValue;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate sequence value for {collectionName}: {ex.Message}",
                ex);
        }
    }
}
