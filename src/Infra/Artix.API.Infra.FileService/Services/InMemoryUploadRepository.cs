namespace Artix.API.Infra.FileService.Services;

using System.Collections.Concurrent;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;

public class InMemoryUploadRepository : IUploadRepository
{
    private readonly ConcurrentDictionary<Guid, UploadSession> _store = new();

    public Task AddAsync(UploadSession session, CancellationToken cancellationToken = default)
    {
        this._store[session.Id] = session;
        return Task.CompletedTask;
    }

    public Task<UploadSession?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        this._store.TryGetValue(id, out var s);
        return Task.FromResult(s);
    }

    public Task UpdateAsync(UploadSession session, CancellationToken cancellationToken = default)
    {
        this._store[session.Id] = session;
        return Task.CompletedTask;
    }
}
