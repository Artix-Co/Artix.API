namespace Artix.API.Core.Contract.Primitives.Repositories;

public interface IRepository<T> where T : class
{
    #region Sync methods

    T GetById(Guid id);
    int Commit();
    void Add(T entity);
    void AddRange(IEnumerable<T> entity);
    void Update(T entity);
    void Delete(T entity);

    #endregion


    #region Asunc methods

    Task<T> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entity);
    Task<int> CommitAsync();
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);

    #endregion

    void BeginTransaction();
    void CommitTransaction();
    void RollbackTransaction();
}
