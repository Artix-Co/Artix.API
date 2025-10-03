namespace Artix.API.Infra.Mongo.Data.Interceptors;

using Core.Domain.Entities.Common;

public interface IMongoInterceptor
{
    void BeforeInsert<T>(T document) where T : AggregateRoot;
    void BeforeUpdate<T>(T document) where T : AggregateRoot;
    Task BeforeInsertAsync<T>(T document, CancellationToken cancellationToken) where T : AggregateRoot;
    Task BeforeUpdateAsync<T>(T document, CancellationToken cancellationToken) where T : AggregateRoot;
}
