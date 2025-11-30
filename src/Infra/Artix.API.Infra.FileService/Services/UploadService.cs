namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;

public class UploadService : IUploadService
{
    private readonly IUploadRepository _repo;
    private readonly IFileStorage _storage;

    public UploadService(IUploadRepository repo, IFileStorage storage)
    {
        this._repo = repo;
        this._storage = storage;
    }

    public async Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize,
        CancellationToken cancellationToken = default)
    {
        await this._storage.EnsureDirectoriesAsync(cancellationToken);
        var session = new UploadSession
        {
            Id = Guid.NewGuid(),
            FileName = Path.GetFileName(fileName),
            TotalSize = totalSize,
            ChunkSize = chunkSize,
            TotalChunks = (int)Math.Ceiling((double)totalSize / chunkSize),
            TempFolder = await this._storage.GetTempFolderAsync(Guid.NewGuid(), cancellationToken)
        };
        session.TempFolder = await this._storage.GetTempFolderAsync(session.Id, cancellationToken);
        await this._repo.AddAsync(session, cancellationToken);
        return session;
    }

    public async Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex,
        CancellationToken cancellationToken = default)
    {
        var s = await this._repo.GetAsync(uploadId, cancellationToken);
        if (s == null) throw new InvalidOperationException("not found");
        s.ReceivedChunks[chunkIndex] = true;
        await this._repo.UpdateAsync(s, cancellationToken);
    }

    public async Task MergeChunksAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        var s = await this._repo.GetAsync(uploadId, cancellationToken);
        if (s == null) throw new InvalidOperationException("not found");
        await using var ms = new MemoryStream();
        await this._storage.MergeAsync(uploadId, s.FileName, s.TotalChunks, ms, cancellationToken);
        s.Completed = true;
        await this._repo.UpdateAsync(s, cancellationToken);
    }

    public Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        return this._repo.GetAsync(uploadId, cancellationToken);
    }
}
