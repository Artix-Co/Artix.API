namespace Artix.API.Core.Contract.Features.Collections;

using Domain.Entities.Collection;
using Primitives.Repositories;

public interface ICollectionCommandRepository : ICommandRepository<Collection>
{
}
