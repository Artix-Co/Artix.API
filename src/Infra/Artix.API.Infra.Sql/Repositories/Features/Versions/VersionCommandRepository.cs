namespace Artix.API.Infra.Sql.Repositories.Features.Versions;

using Core.Contract.Features.Versions.Commands;
using Core.Domain.Entities.Version;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class VersionCommandRepository : CommandRepository<AppVersion>, IVersionCommandRepository
{
    private readonly ILogger<VersionCommandRepository> _logger;

    public VersionCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<VersionCommandRepository> logger)
        : base(commandDbContext)
    {
        _logger = logger;
    }
}
