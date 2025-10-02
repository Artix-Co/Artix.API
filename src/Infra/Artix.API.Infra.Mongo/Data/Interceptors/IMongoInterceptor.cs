namespace Artix.API.Infra.Mongo.Data.Interceptors;

using Core.Domain.Entities.Common;

public interface IMongoInterceptor
{
    void BeforeInsert<T>(T document) where T : BaseEntity;
    void BeforeUpdate<T>(T document) where T : BaseEntity;
    Task BeforeInsertAsync<T>(T document, CancellationToken cancellationToken) where T : BaseEntity;
    Task BeforeUpdateAsync<T>(T document, CancellationToken cancellationToken) where T : BaseEntity;
}
