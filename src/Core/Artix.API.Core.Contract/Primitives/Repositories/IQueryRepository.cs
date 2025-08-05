namespace Artix.API.Core.Contract.Primitives.Repositories;

using Domain.Entities.Common;

public interface IQueryRepository<T> where T : BaseEntity
{
    T? GetById(long id);
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
