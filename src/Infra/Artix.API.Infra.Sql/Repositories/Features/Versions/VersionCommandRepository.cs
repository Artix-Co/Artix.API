namespace Artix.API.Infra.Sql.Repositories.Features.Versions;

using Core.Contract.Features.Versions.Commands;
using Core.Domain.Entities.Version;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class VersionCommandRepository : CommandRepository<AppVersion>, IVersionCommandRepository
{
    public VersionCommandRepository(ArtixCommandDbContext commandDbContext,
        ILogger<CommandRepository<AppVersion>> logger)
        : base(commandDbContext, logger)
    {
    }
}
