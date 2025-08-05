namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Objects.Commands;
using Core.Domain.Entities.Museum;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class ObjectCommandRepository : CommandRepository<Object>, IObjectCommandRepository
{
    private readonly ILogger<ObjectCommandRepository> _logger;
    private readonly ArtixCommandDbContext _commandDbContext;

    public ObjectCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<ObjectCommandRepository> logger)
        : base(commandDbContext)
    {
        _logger = logger;
        _commandDbContext = commandDbContext;
    }
}
