namespace Artix.API.Core.Contract.Features.Museums.Client.Queries;

using Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginate;
using Domain.Entities.Museum;
using GetAll;
using GetDetailByIds;
using GetJournalEntries;
using GetKeyStatus;
using GetObjects;
using Museums.Queries.GetObjects;
using Primitives.Models;
using Primitives.Repositories;

public interface IMuseumQueryRepository : IQueryRepository<Museum>
{
    IEnumerable<AllMuseumsDto> GetAllMuseumsClient(GetAllMuseumsQuery dto);

    MuseumDetailsByIdDto GetDetailsById(GetMuseumDetailsByIdQuery dto);

    IEnumerable<MuseumObjectDto> GetObjects(GetMuseumObjectsQuery dto);

    IEnumerable<MuseumJournalEntryDto> GetJournalEntries(GetMuseumJournalEntriesQuery dto);

    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default);


    Task<PaginatedResult<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<PaginatedMuseumsDto>> GetAllMuseumsAdminAsync(GetPaginateMuseumsQuery dto, CancellationToken cancellationToken=default);
}
