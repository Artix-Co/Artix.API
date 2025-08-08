namespace Artix.API.Core.Contract.Features.Collections.Commands;

using Domain.Entities.Collection;
using Primitives.Repositories;

public interface ICollectionCommandRepository : ICommandRepository<Collection>
{
}
