namespace Artix.API.Core.Contract.Features.Objects.Commands;

using Domain.Entities.Museum;
using Primitives.Repositories;

public interface IObjectCommandRepository : ICommandRepository<MuseumObject>
{
}
