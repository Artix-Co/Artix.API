namespace Artix.API.Core.Contract.Features.Files.Commands;

using Domain.Entities.File;
using Primitives.Repositories;

public interface IFileCommandRepository : ICommandRepository<FileEntity>
{
}
