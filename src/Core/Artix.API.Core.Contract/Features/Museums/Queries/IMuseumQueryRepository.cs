namespace Artix.API.Core.Contract.Features.Museums.Queries;

using Domain.Entities.Museum;
using GetAll;
using GetDetailByIds;
using GetMuseumJournalEntries;
using GetMuseumKeyStatus;
using GetMuseumObjects;
using GetObjects;
using Primitives.Models;
using Primitives.Repositories;

public interface IMuseumQueryRepository : IQueryRepository<Museum>
{
    IEnumerable<AllMuseumDto> GetAll(GetAllMuseumsQuery dto);

    Task<MuseumDetailsByIdDto> GetDetailsByIdAsync(GetMuseumDetailsByIdQuery dto,
        CancellationToken cancellationToken = default);

    IEnumerable<MuseumObjectDto> GetObjects(GetMuseumObjectsQuery dto);

    IEnumerable<MuseumJournalEntryDto> GetJournalEntries(GetMuseumJournalEntriesQuery dto);

    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default);


    Task<PagedData<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default);
}
