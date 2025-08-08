namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities.Common;

public interface ICommandRepository<T> where T : BaseEntity
{
    void InsertRange(IEnumerable<T> entities);
    void Insert(T entity);
    void Update(T entity);
    void Delete(long id);
    T? GetById(long id);

    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    Task InsertAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}
