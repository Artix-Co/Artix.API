namespace Artix.API.Infra.Sql.Primitives;

using System.Linq.Expressions;
using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data.DbContexts;
using Microsoft.Extensions.Logging;

public class QueryRepository<T> : IQueryRepository<T> where T : AggregateRoot
{
    protected readonly ArtixQueryDbContext _queryDbContext;
    protected readonly ILogger<QueryRepository<T>> _logger;
    
    public QueryRepository(
        ArtixQueryDbContext queryDbContext,
        ILogger<QueryRepository<T>> logger)
    {
        _queryDbContext = queryDbContext;
        _logger = logger;
    }


    protected IQueryable<TOut> Project<TOut>(
        IQueryable<T> query,
        Expression<Func<T, TOut>> selector)
    {
        return query.Select(selector);
    }
}
