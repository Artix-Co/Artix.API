namespace Artix.API.Infra.Sql.Primitives;

using Core.Contract.Primitives.Repositories;
using Core.Domain.Entities.Common;
using Data;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;

public class QueryRepository<T> : IQueryRepository<T> where T : AggregateRoot
{
    protected readonly ArtixQueryDbContext _queryDbContext;

    public QueryRepository(ArtixQueryDbContext queryDbContext)
    {
        _queryDbContext = queryDbContext;
    }
    
}
