namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities.Common;

public interface IRepository<T> where T : AggregateRoot
{
    #region Sync methods

    T? GetById(Guid id);
    int Commit();
    void Add(T entity);
    void AddRange(IEnumerable<T> entity);
    void Update(T entity);
    void Delete(T entity);

    #endregion


    #region Asunc methods

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entity, CancellationToken cancellationToken = default);
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    #endregion

    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}
