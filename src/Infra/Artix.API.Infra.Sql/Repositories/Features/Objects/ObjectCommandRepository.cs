namespace Artix.API.Infra.Sql.Repositories.Features.Objects;

using Core.Contract.Features.Objects.Commands;
using Core.Domain.Entities.Museum;
using Core.Domain.Entities.Object;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class ObjectCommandRepository : CommandRepository<Object>, IObjectCommandRepository
{
    public ObjectCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<CommandRepository<Object>> logger)
        : base(commandDbContext, logger)
    {
    }
}
