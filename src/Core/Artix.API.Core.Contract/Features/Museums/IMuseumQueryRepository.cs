namespace Artix.API.Core.Contract.Features.Museums;

using Admin.Queries.GetPaginateMuseums;
using Admin.Queries.GetPaginateObjects;
using Client.Queries.GetAll;
using Client.Queries.GetDetailByIds;
using Client.Queries.GetJournalEntries;
using Client.Queries.GetKeyStatus;
using Client.Queries.GetObjects;
using Primitives.Models;
using Primitives.Repositories;
using Domain.Entities.Museum;
using Objects.Client.Queries.GetPaginateObjects;

public interface IMuseumQueryRepository : IQueryRepository<Museum>
{
    IEnumerable<ClientAllMuseumsDto> GetAllMuseumsClient(GetClientAllMuseumsQuery dto);

    ClientMuseumDetailsByIdDto GetDetailsById(GetClientMuseumDetailsByIdQuery dto);

    IEnumerable<ClientMuseumObjectDto> GetObjects(GetClientMuseumObjectsQuery dto);

    Task<PaginatedResult<AdminMuseumObjectDto>> GetAdminObjectsAsync(GetAdminMuseumObjectsQuery dto,
        CancellationToken cancellationToken = default);

    IEnumerable<MuseumJournalEntryDto> GetJournalEntries(GetMuseumJournalEntriesQuery dto);

    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default);


    Task<PaginatedResult<ClientPaginateObjectsDto>> GetAllObjectsAsync(GetClientPaginateObjectsQuery dto,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<AdminPaginatedMuseumsDto>> GetAllMuseumsAdminAsync(GetAdminPaginateMuseumsQuery dto,
        CancellationToken cancellationToken = default);
}
