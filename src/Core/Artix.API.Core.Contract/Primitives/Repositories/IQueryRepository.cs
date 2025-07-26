namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities.Common;

public interface IQueryRepository<T> where T : IAggregateRoot
{
    T GetById(long id, Func<IQueryable<T>, IQueryable<T>> include = null);

    Task<T> GetByIdAsync(long id, CancellationToken cancellationToken = default,
        Func<IQueryable<T>, IQueryable<T>> include = null);
}
