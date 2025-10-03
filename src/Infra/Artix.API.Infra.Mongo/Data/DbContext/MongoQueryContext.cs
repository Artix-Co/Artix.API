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
        if (string.IsNullOrEmpty(name))
            throw new ArgumentNullException(nameof(name));
        return _database.GetCollection<T>(name);
    }

    // Generic find method with filter, sort, and options
    public async Task<List<T>> FindAsync<T>(
        FilterDefinition<T> filter,
        SortDefinition<T>? sort = null,
        int? limit = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        var collection = GetCollection<T>(typeof(T).Name + "s");

        // Combine with default IsDeleted filter
        var finalFilter = Builders<T>.Filter.And(
            filter,
            Builders<T>.Filter.Eq(q => q.IsDeleted, false));

        var findOptions = new FindOptions<T>();
        if (sort != null)
            findOptions.Sort = sort;
        if (limit.HasValue)
            findOptions.Limit = limit.Value;

        return await collection.Find(finalFilter).ToListAsync(cancellationToken);
    }

    // Find a single document
    public async Task<T> FindOneAsync<T>(
        FilterDefinition<T> filter,
        SortDefinition<T>? sort = null,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        var collection = GetCollection<T>(typeof(T).Name + "s");

        // Combine with default IsDeleted filter
        var finalFilter = Builders<T>.Filter.And(
            filter,
            Builders<T>.Filter.Eq(q => q.IsDeleted, false));

        var findOptions = new FindOptions<T>();
        if (sort != null)
            findOptions.Sort = sort;

        return await collection.Find(finalFilter).FirstOrDefaultAsync(cancellationToken);
    }

    // Count documents
    public async Task<long> CountAsync<T>(
        FilterDefinition<T> filter,
        CancellationToken cancellationToken = default) where T : AggregateRoot
    {
        var collection = GetCollection<T>(typeof(T).Name + "s");

        // Combine with default IsDeleted filter
        var finalFilter = Builders<T>.Filter.And(
            filter,
            Builders<T>.Filter.Eq(q => q.IsDeleted, false));

        return await collection.CountDocumentsAsync(finalFilter, cancellationToken: cancellationToken);
    }
}
