namespace Artix.API.Core.Contract.Features.Museums.Queries;

using Domain.Entities.Museum;
using GetAll;
using GetById;
using GetMuseumJournalEntries;
using GetMuseumKeyStatus;
using GetMuseumObjects;
using GetObjects;
using Primitives.Models;
using Primitives.Repositories;

public interface IMuseumQueryRepository : IQueryRepository<Museum>
{
    Task<IEnumerable<AllMuseumDto>>
        GetAllAsync(GetAllMuseumsQuery dto, CancellationToken cancellationToken = default);

    Task<MuseumByIdDto?> GetDetailsByIdAsync(GetMuseumByIdQuery dto, CancellationToken cancellationToken = default);

    Task<IEnumerable<MuseumObjectDto>> GetObjectsAsync(GetMuseumObjectsQuery dto,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<MuseumJournalEntryDto>> GetJournalEntriesAsync(GetMuseumJournalEntriesQuery dto,
        CancellationToken cancellationToken = default);

    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery dto,
        CancellationToken cancellationToken = default);


    Task<PagedData<AllObjectDto>> GetAllObjectsAsync(GetAllObjectsQuery dto,
        CancellationToken cancellationToken = default);


}
