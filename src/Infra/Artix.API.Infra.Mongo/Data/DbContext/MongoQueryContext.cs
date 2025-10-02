namespace Artix.API.Infra.Mongo.Data.DbContext;

using Core.Domain.Entities.Common;
using MongoDB.Driver;

public sealed class MongoQueryContext
{
    private readonly IMongoDatabase _database;

    public MongoQueryContext(IMongoDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    // Get a collection by name
    public IMongoCollection<T> GetCollection<T>(string name) where T : AggregateRoot
    {
        return _database.GetCollection<T>(name ?? throw new ArgumentNullException(nameof(name)));
    }

    // Find documents (async)
    public async Task<List<T>> FindAsync<T>(CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        var filter = Builders<T>.Filter.Eq("IsDeleted", false);
        return await collection.Find(filter).ToListAsync(cancellationToken);
    }

    // Find a single document (async)
    public async Task<T> FindOneAsync<T>(CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        var filter = Builders<T>.Filter.Eq("IsDeleted", false);
        return await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    // Count documents (async)
    public async Task<long> CountAsync<T>(CancellationToken cancellationToken = default)
        where T : AggregateRoot
    {
        var collection = _database.GetCollection<T>(typeof(T).Name + "s");
        var filter = Builders<T>.Filter.Eq("IsDeleted", false);
        return await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}
