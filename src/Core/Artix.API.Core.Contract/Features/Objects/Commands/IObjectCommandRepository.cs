namespace Artix.API.Core.Contract.Features.Objects.Commands;

using Domain.Entities.Museum;
using Domain.Entities.Object;
using Primitives.Repositories;

public interface IObjectCommandRepository : ICommandRepository<Object>
{
}
