namespace Artix.API.Core.Contract.Primitives.Repositories;


public interface IUnitOfWork : IDisposable
{
    #region Sync Methods
    void BeginTransaction();
    void Commit();
    void Rollback();
    #endregion

    #region Async Methods
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollbackAsync(CancellationToken cancellationToken = default);
    #endregion
}
