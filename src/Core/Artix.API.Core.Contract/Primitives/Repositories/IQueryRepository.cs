namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities.Common;

public interface IQueryRepository<T> where T : IAggregateRoot
{
    T? GetById(long id);

    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    
    
    T? GetGraphById(long id);
    Task<T?> GetGraphByIdAsync(long id, CancellationToken cancellationToken = default);
}
