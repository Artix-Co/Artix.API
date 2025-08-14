namespace Artix.API.Core.Contract.Features.Versions.Commands;

using Domain.Entities.Version;
using Primitives.Repositories;

public interface IVersionCommandRepository : ICommandRepository<AppVersion>
{
}
