namespace Artix.API.Core.Contract.Features.Museums;

using Artix.API.Core.Contract.Features.Museums.Admin.Queries.GetPaginate;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetAll;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetDetailByIds;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetJournalEntries;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetKeyStatus;
using Artix.API.Core.Contract.Features.Museums.Client.Queries.GetObjects;
using Queries.GetObjects;
using Primitives.Models;
using Primitives.Repositories;
using Domain.Entities.Museum;

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
