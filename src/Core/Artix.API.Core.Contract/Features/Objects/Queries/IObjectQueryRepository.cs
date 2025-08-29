namespace Artix.API.Core.Contract.Features.Objects.Queries;

using Domain.Entities.Object;
using GetAllObjectsAdmins;
using GetObjectDetailsByIdAdmins;
using GetObjectDetailsByIdClients;
using Primitives.Models;
using Primitives.Repositories;

public interface IObjectQueryRepository : IQueryRepository<Object>
{
    Task<ObjectDetailsByIdClientDto> GetDetailsByIdAsync(GetObjectDetailsByIdClientQuery clientQuery,
        CancellationToken cancellationToken = default);
    
    Task<PaginatedResult<AllObjectsAdminDto>> GetAllObjectsAdminAsync(GetAllObjectsAdminQuery query,
        CancellationToken cancellationToken = default);
    
    
    Task<ObjectDetailsByIdAdminDto> GetAllObjectDetailsByIdAdminAsync(GetObjectDetailsByIdAdminQuery query,
        CancellationToken cancellationToken = default);
}
