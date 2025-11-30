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

    public async Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize)
    {
        await this._storage.EnsureDirectoriesAsync();
        var session = new UploadSession
        {
            Id = Guid.NewGuid(),
            FileName = Path.GetFileName(fileName),
            TotalSize = totalSize,
            ChunkSize = chunkSize,
            TotalChunks = (int)Math.Ceiling((double)totalSize / chunkSize),
            TempFolder = await this._storage.GetTempFolderAsync(Guid.NewGuid())
        };
        session.TempFolder = await this._storage.GetTempFolderAsync(session.Id);
        await this._repo.AddAsync(session);
        return session;
    }

    public async Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex)
    {
        var s = await this._repo.GetAsync(uploadId);
        if (s == null) throw new InvalidOperationException("not found");
        s.ReceivedChunks[chunkIndex] = true;
        await this._repo.UpdateAsync(s);
    }

    public async Task MergeChunksAsync(Guid uploadId)
    {
        var s = await this._repo.GetAsync(uploadId);
        if (s == null) throw new InvalidOperationException("not found");
        await using var ms = new MemoryStream();
        await this._storage.MergeAsync(uploadId, s.FileName, s.TotalChunks, ms, CancellationToken.None);
        s.Completed = true;
        await this._repo.UpdateAsync(s);
    }

    public Task<UploadSession> GetStatusAsync(Guid uploadId)
    {
        return this._repo.GetAsync(uploadId);
    }
}
