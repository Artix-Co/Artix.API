namespace Artix.API.Core.Contract.Features.Objects;

 
using Primitives.Repositories;
using Artix.API.Core.Domain.Entities.Object;
using GetObjectDetailsByIdQuery = Client.Queries.GetObjectDetailsById.GetObjectDetailsByIdQuery;
using ObjectDetailsByIdDto = Client.Queries.GetObjectDetailsById.ObjectDetailsByIdDto;

public interface IObjectQueryRepository : IQueryRepository<Object>
{
    Task<ObjectDetailsByIdDto> GetDetailsByIdAsync(GetObjectDetailsByIdQuery query,
        CancellationToken cancellationToken = default);
    
 
    
    
    Task<Admin.Queries.GetObjectDetailsById.ObjectDetailsByIdDto> GetObjectDetailsByIdAdminAsync(Admin.Queries.GetObjectDetailsById.GetObjectDetailsByIdQuery query,
        CancellationToken cancellationToken = default);
}
