namespace Artix.API.Core.Contract.Features.Objects;

 
using Primitives.Repositories;
using Artix.API.Core.Domain.Entities.Object;
using Client.Queries.GetObjectDetailsById;

public interface IObjectQueryRepository : IQueryRepository<Object>
{
    Task<ClientObjectDetailsByIdDto> GetDetailsByIdAsync(GetClientObjectDetailsByIdQuery query,
        CancellationToken cancellationToken = default);
    
 
    
    
    Task<Admin.Queries.GetObjectDetailsById.AdminObjectDetailsByIdDto> GetObjectDetailsByIdAdminAsync(Admin.Queries.GetObjectDetailsById.GetAdminObjectDetailsByIdQuery query,
        CancellationToken cancellationToken = default);
}
