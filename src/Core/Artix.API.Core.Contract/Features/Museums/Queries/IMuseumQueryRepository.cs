namespace Artix.API.Core.Contract.Features.Museums.Queries;

using Domain.Entities.Museum;
using GetAll;
using GetById;
using GetMuseumJournalEntries;
using GetMuseumKeyStatus;
using GetMuseumObjects;
using Primitives.Repositories;

public interface IMuseumQueryRepository : IQueryRepository<Museum>
{
    Task<IEnumerable<AllMuseumDto>> GetAllAsync(GetAllMuseumsQuery query, CancellationToken cancellationToken = default);
    Task<MuseumByIdDto?> GetDetailsByIdAsync(GetMuseumByIdQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<MuseumObjectDto>> GetObjectsAsync(GetMuseumObjectsQuery query, CancellationToken cancellationToken = default);
    Task<IEnumerable<MuseumJournalEntryDto>> GetJournalEntriesAsync(GetMuseumJournalEntriesQuery query, CancellationToken cancellationToken = default);
    Task<MuseumKeyStatusDto?> GetKeyStatusAsync(GetMuseumKeyStatusQuery query, CancellationToken cancellationToken = default);
}

