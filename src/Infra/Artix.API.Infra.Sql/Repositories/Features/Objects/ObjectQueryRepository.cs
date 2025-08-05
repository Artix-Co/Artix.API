namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Objects.Queries;
using Core.Domain.Entities.Museum;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class ObjectQueryRepository : QueryRepository<Object>, IObjectQueryRepository
{
    private readonly ILogger<ObjectQueryRepository> _logger;
    private readonly ArtixQueryDbContext _queryDbContext;

    public ObjectQueryRepository(ArtixQueryDbContext queryDbContext, ILogger<ObjectQueryRepository> logger)
        : base(queryDbContext)
    {
        _logger = logger;
        _queryDbContext = queryDbContext;
    }
}
