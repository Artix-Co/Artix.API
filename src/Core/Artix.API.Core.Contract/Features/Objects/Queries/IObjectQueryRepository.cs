namespace Artix.API.Core.Contract.Features.Objects.Queries;

using Domain.Entities.Museum;
using Domain.Entities.Object;
using GetDetailByIds;
using Primitives.Repositories;

public interface IObjectQueryRepository : IQueryRepository<Object>
{
    Task<ObjectDetailByIdDto> GetDetailsByIdAsync(GetObjectDetailByIdQuery query,
        CancellationToken cancellationToken = default);
}
