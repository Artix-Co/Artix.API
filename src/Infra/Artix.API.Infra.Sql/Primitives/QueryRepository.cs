namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;

public class QueryRepository<T> : IQueryRepository<T> where T : class, IAggregateRoot
{
    private readonly DbContext _context;

    public QueryRepository(DbContext context)
    {
        _context = context;
    }

    public T? GetById(long id)
    {
        return _context.Set<T>().Find(id);
    }

    public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, cancellationToken);
    }

    public T? GetGraphById(long id)
    {
        var query = IncludeRequiredNavigations(_context.Set<T>());
        return query.FirstOrDefault(e => EF.Property<long>(e, "Id") == id);
    }

    public async Task<T?> GetGraphByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var query = IncludeRequiredNavigations(_context.Set<T>());
        var entity = await query.FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id, cancellationToken);

        if (entity is BaseAggregateRoot aggregateRoot)
        {
            aggregateRoot.LoadEntitiesFromGraph();
        }

        return entity;
    }

    private IQueryable<T?> IncludeRequiredNavigations(IQueryable<T> query)
    {
        var entityType = _context.Model.FindEntityType(typeof(T));

        var requiredNavigations = entityType?
            .GetNavigations()
            .Where(n => !n.IsOnDependent && !n.IsCollection &&
                        !n.ForeignKey.IsRequiredDependent) // exclude optional/collection
            .ToList();

        foreach (var navigation in requiredNavigations)
        {
            query = query.Include(navigation.Name);
        }

        return query;
    }
}
