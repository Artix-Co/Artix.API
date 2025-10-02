namespace Artix.API.Infra.Mongo.Data.Interceptors;

using Core.Domain.Entities.Common;

public sealed class MongoTimestampInterceptor : IMongoInterceptor
{
    // Interface to define interceptor methods
  

    // Synchronous method for insert operations
    public void BeforeInsert<T>(T document) where T : BaseEntity
    {
        if (document == null) return;

        var now = DateTime.UtcNow;
        document.CreatedAt = now;
        document.ModifiedAt = now;
    }

    // Synchronous method for update operations
    public void BeforeUpdate<T>(T document) where T : BaseEntity
    {
        if (document == null) return;

        document.ModifiedAt = DateTime.UtcNow;
        
    }

    // Asynchronous method for insert operations
    public async Task BeforeInsertAsync<T>(T document, CancellationToken cancellationToken) where T : BaseEntity
    {
        BeforeInsert(document); // Reuse synchronous logic
        await Task.CompletedTask; // No additional async logic
    }

    // Asynchronous method for update operations
    public async Task BeforeUpdateAsync<T>(T document, CancellationToken cancellationToken) where T : BaseEntity
    {
        BeforeUpdate(document); // Reuse synchronous logic
        await Task.CompletedTask; // No additional async logic
    }
}
