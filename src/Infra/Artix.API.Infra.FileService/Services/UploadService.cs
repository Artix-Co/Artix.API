namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;
using Microsoft.Extensions.Options;

public class UploadService : IUploadService
{
    private readonly IUploadRepository _repo;
    private readonly IFileStorage _storage;
    private readonly FileSettings _fileSettings;

    public UploadService(IUploadRepository repo, IFileStorage storage, IOptions<FileSettings> fileSettings)
    {
        _repo = repo;
        _storage = storage;
        _fileSettings = fileSettings.Value;
    }

    public async Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize, CancellationToken cancellationToken = default)
    {
        await _storage.EnsureDirectoriesAsync(cancellationToken);
        var session = new UploadSession
        {
            Id = Guid.NewGuid(),
            FileName = Path.GetFileName(fileName),
            TotalSize = totalSize,
            ChunkSize = chunkSize,
            TotalChunks = (int)Math.Ceiling((double)totalSize / chunkSize),
            TempFolder = await _storage.GetTempFolderAsync(Guid.NewGuid(), cancellationToken)
        };
        session.TempFolder = await _storage.GetTempFolderAsync(session.Id, cancellationToken);
        await _repo.AddAsync(session, cancellationToken);
        return session;
    }

    public async Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex, CancellationToken cancellationToken = default)
    {
        var s = await _repo.GetAsync(uploadId, cancellationToken);
        if (s == null) throw new InvalidOperationException("not found");
        s.ReceivedChunks[chunkIndex] = true;
        await _repo.UpdateAsync(s, cancellationToken);
    }

    public async Task MergeChunksAsync(Guid uploadId, CancellationToken ct = default)
    {
        var session = await _repo.GetAsync(uploadId, ct) ?? throw new InvalidOperationException("Upload session not found.");

        var receivedCount = session.ReceivedChunks?.Count(kvp => kvp.Value) ?? 0;
        if (receivedCount != session.TotalChunks)
            throw new InvalidOperationException($"Cannot merge: only {receivedCount} of {session.TotalChunks} chunks received.");

        if (session.Completed)
            return;

        var finalPath = await _storage.MergeAsync(uploadId, session.FileName, session.TotalChunks, ct);

        session.MergedFilePath = finalPath;
        session.FinalFileName = Path.GetFileName(finalPath);
        session.Completed = true;

        await _repo.UpdateAsync(session, ct);
    }

    public Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        return _repo.GetAsync(uploadId, cancellationToken);
    }
}
