namespace Artix.API.Infra.Sql.Repositories.Features.Collections;

using Core.Contract.Features.Collections.Commands;
using Core.Domain.Entities.Collection;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class CollectionCommandRepository : CommandRepository<Collection>, ICollectionCommandRepository
{
    public CollectionCommandRepository(ArtixCommandDbContext commandDbContext,
        ILogger<CollectionCommandRepository> logger)
        : base(commandDbContext)
    {
    }
}
