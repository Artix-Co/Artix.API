namespace Artix.API.Core.Contract.Features.Objects.Queries;

using Domain.Entities.Museum;
using Primitives.Repositories;

public interface IObjectQueryRepository : IQueryRepository<MuseumObject>
{
}
