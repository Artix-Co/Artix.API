namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;
using Microsoft.Extensions.Options;
public sealed class UploadService : IUploadService
{
    private readonly IUploadRepository _repo;
    private readonly IFileStorage _storage;
    private readonly FileSettings _fileSettings;

    public UploadService(
        IUploadRepository repo,
        IFileStorage storage,
        IOptions<FileSettings> fileSettings)
    {
        _repo = repo;
        _storage = storage;
        _fileSettings = fileSettings.Value;
    }

    public async Task<UploadSession> InitiateAsync(
        string fileName,
        long totalSize,
        int chunkSize,
        CancellationToken cancellationToken = default)
    {
        await _storage.EnsureDirectoriesAsync(cancellationToken);

        var sessionId = Guid.NewGuid();

        var session = new UploadSession
        {
            Id = sessionId,
            FileName = Path.GetFileName(fileName),
            TotalSize = totalSize,
            ChunkSize = chunkSize,
            TotalChunks = (int)Math.Ceiling((double)totalSize / chunkSize),

            // فقط یک بار ساخته می‌شود و بر اساس sessionId
            TempFolder = await _storage.GetTempFolderAsync(sessionId, cancellationToken)
        };

        await _repo.AddAsync(session, cancellationToken);
        return session;
    }

    public async Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex,
        CancellationToken cancellationToken = default)
    {
        var session = await _repo.GetAsync(uploadId, cancellationToken)
                     ?? throw new InvalidOperationException("Upload session not found.");

        session.ReceivedChunks[chunkIndex] = true;
        await _repo.UpdateAsync(session, cancellationToken);
    }

    public async Task MergeChunksAsync(Guid uploadId, CancellationToken ct = default)
    {
        var session = await _repo.GetAsync(uploadId, ct)
                     ?? throw new InvalidOperationException("Upload session not found.");

        var receivedCount = session.ReceivedChunks.Count(x => x.Value);
        if (receivedCount != session.TotalChunks)
            throw new InvalidOperationException(
                $"Cannot merge: only {receivedCount} of {session.TotalChunks} chunks received.");

        if (session.Completed)
            return;

        // ⚡ فایل واقعی (نه gzip) ساخته می‌شود
        var physicalPath = await _storage.MergeAsync(
            uploadId,
            session.FileName,
            session.TotalChunks,
            ct);

        session.PhysicalFilePath = physicalPath;

        // ⚡ مسیر پایدار برای استفاده از StaticFiles / files/{name}
        // مهم → نباید Path.Combine استفاده شود
        session.VirtualFilePath =
            $"/files/{session.FileName}".Replace("\\", "/");

        session.Completed = true;

        await _repo.UpdateAsync(session, ct);
    }

    public Task<UploadSession> GetStatusAsync(Guid uploadId,
        CancellationToken cancellationToken = default)
        => _repo.GetAsync(uploadId, cancellationToken);
}
