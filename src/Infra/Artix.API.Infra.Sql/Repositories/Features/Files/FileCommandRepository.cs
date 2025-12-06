namespace Artix.API.Infra.Sql.Repositories.Features.Files;

using Core.Contract.Features.Files.Commands;
using Core.Domain.Entities.File;
using Data.DbContexts;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class FileCommandRepository : CommandRepository<FileEntity>, IFileCommandRepository
{
    public FileCommandRepository(ArtixCommandDbContext commandDbContext, ILogger<CommandRepository<FileEntity>> logger)
        : base(commandDbContext, logger)
    {
    }
}
