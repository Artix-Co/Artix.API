namespace Artix.API.Infra.Mongo.Primitives;

using Artix.API.Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

 

public class MongoQueryRepository<T> : IQueryRepository<T> where T : AggregateRoot
{
    protected readonly IMongoCollection<T> _collection;
    protected readonly ILogger<MongoQueryRepository<T>> _logger;

    public MongoQueryRepository(IMongoDatabase database, ILogger<MongoQueryRepository<T>> logger)
    {
        _collection = database.GetCollection<T>(typeof(T).Name.ToLowerInvariant());
        _logger = logger;
    }
}
