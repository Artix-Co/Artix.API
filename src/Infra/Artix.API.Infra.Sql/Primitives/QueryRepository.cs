namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities._primitives;
using Data;
using Exceptions;
using Microsoft.EntityFrameworkCore;

public class QueryRepository<T>(ArtixQueryDbContext queryDbContext) : IQueryRepository<T> where T : BaseEntity
{
    protected readonly ArtixQueryDbContext _queryDbContext = queryDbContext;

    #region Sync Methods

    public T GetById(long id, Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        IQueryable<T> query = this._queryDbContext.Set<T>();

        if (include != null)
        {
            query = include(query);
        }

        var entity = query.FirstOrDefault(e => e.Id == id);

        if (entity == null)
        {
            throw InfrastructureException.DatabaseError($"{typeof(T).Name} with ID {id} was not found.");
        }

        return entity;
    }

    #endregion

    #region Async Methods

    public async Task<T> GetByIdAsync(long id, Func<IQueryable<T>, IQueryable<T>> include = null)
    {
        IQueryable<T> query = this._queryDbContext.Set<T>();

        if (include != null)
        {
            query = include(query);
        }

        var entity = await query.FirstOrDefaultAsync(e => e.Id == id);

        if (entity == null)
        {
            throw InfrastructureException.DatabaseError($"{typeof(T).Name} with ID {id} was not found.");
        }

        return entity;
    }

    #endregion
}
