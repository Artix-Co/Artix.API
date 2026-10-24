namespace Artix.API.Infra.FileService.Services;

using System.Collections.Concurrent;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;

public class InMemoryUploadRepository : IUploadRepository
{
    private readonly ConcurrentDictionary<Guid, UploadSession> _store = new(EqualityComparer<Guid>.Default);
    public Task AddAsync(UploadSession session, CancellationToken ct = default)
    {
        _store.TryAdd(session.Id, session);
        return Task.CompletedTask;
    }
    public Task<UploadSession?> GetAsync(Guid id, CancellationToken ct = default)
    {
        _store.TryGetValue(id, out var session);
        return Task.FromResult(session);
    }
    public Task UpdateAsync(UploadSession session, CancellationToken ct = default)
    {
        _store[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<UploadSession>> RemoveExpiredAsync(DateTime cutoff, CancellationToken ct = default)
    {
        var expired = _store.Values.Where(x => !x.Completed && x.CreatedAt <= cutoff).ToArray();
        foreach (var session in expired)
            _store.TryRemove(session.Id, out _);

        return Task.FromResult<IReadOnlyCollection<UploadSession>>(expired);
    }
}
