namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities._primitives;

public interface IQueryRepository<T> where T : BaseEntity
{
    T GetById(long id, Func<IQueryable<T>, IQueryable<T>> include = null);

    Task<T> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>> include = null);
}
