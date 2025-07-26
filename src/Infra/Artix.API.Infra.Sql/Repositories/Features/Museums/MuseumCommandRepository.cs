namespace Artix.API.Infra.Sql.Repositories.Features.Museums;

using Core.Contract.Features.Museums.Commands;
using Core.Domain.Entities.Museum;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class MuseumCommandRepository : CommandRepository<Museum>, IMuseumCommandRepository
{
    public MuseumCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<MuseumCommandRepository> logger)
        : base(commandDbContext)
    {
    }
 
}
