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
    IEnumerable<AllMuseumsDto> GetAllMuseumsClient(GetAllMuseumsQuery dto);

    MuseumDetailsByIdDto GetDetailsById(GetMuseumDetailsByIdQuery dto);

    IEnumerable<MuseumObjectDto> GetObjects(GetMuseumObjectsQuery dto);

    Task<PaginatedResult<AdminMuseumObjectDto>> GetAdminObjectsAsync(GetAdminMuseumObjectsQuery dto,
        CancellationToken cancellationToken = default);

    IEnumerable<MuseumJournalEntryDto> GetJournalEntries(GetMuseumJournalEntriesQuery dto);

    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default);


    Task<PaginatedResult<PaginateObjectsDto>> GetAllObjectsAsync(GetPaginateObjectsQuery dto,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<PaginatedMuseumsDto>> GetAllMuseumsAdminAsync(GetPaginateMuseumsQuery dto,
        CancellationToken cancellationToken = default);
}
