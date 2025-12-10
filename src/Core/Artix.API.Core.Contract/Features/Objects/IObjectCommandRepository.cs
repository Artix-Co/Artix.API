namespace Artix.API.Core.Contract.Features.Objects;

using Primitives.Repositories;
using Artix.API.Core.Domain.Entities.Object;

public interface IObjectCommandRepository : ICommandRepository<Object>
{
}
