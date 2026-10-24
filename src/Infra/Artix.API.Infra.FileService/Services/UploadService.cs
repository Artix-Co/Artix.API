namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;
using Microsoft.Extensions.Options;
public sealed class UploadService : IUploadService
{
    private const long MaxUploadSize = 100L * 1024 * 1024;
    private const int MinChunkSize = 64 * 1024;
    private const int MaxChunkSize = 8 * 1024 * 1024;
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
        long? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name is required.", nameof(fileName));
        if (totalSize <= 0 || totalSize > MaxUploadSize)
            throw new ArgumentOutOfRangeException(nameof(totalSize));
        if (chunkSize < MinChunkSize || chunkSize > MaxChunkSize)
            throw new ArgumentOutOfRangeException(nameof(chunkSize));

        await _storage.EnsureDirectoriesAsync(cancellationToken);

        var sessionId = Guid.NewGuid();

        var session = new UploadSession
        {
            Id = sessionId,
            OwnerUserId = ownerUserId,
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
        long? ownerUserId = null,
        CancellationToken cancellationToken = default)
    {
        var session = await _repo.GetAsync(uploadId, cancellationToken)
                     ?? throw new InvalidOperationException("Upload session not found.");
        EnsureOwner(session, ownerUserId);
        if (session.Completed || chunkIndex < 0 || chunkIndex >= session.TotalChunks)
            throw new ArgumentOutOfRangeException(nameof(chunkIndex));

        session.ReceivedChunks[chunkIndex] = true;
        await _repo.UpdateAsync(session, cancellationToken);
    }

    public async Task MergeChunksAsync(Guid uploadId, long? ownerUserId = null, CancellationToken ct = default)
    {
        var session = await _repo.GetAsync(uploadId, ct)
                     ?? throw new InvalidOperationException("Upload session not found.");

        EnsureOwner(session, ownerUserId);

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

        if (!await HasExpectedSignatureAsync(physicalPath, session.FileName, ct))
        {
            File.Delete(physicalPath);
            throw new InvalidOperationException("Uploaded file content does not match its allowed type.");
        }

        session.PhysicalFilePath = physicalPath;

        // ⚡ مسیر پایدار برای استفاده از StaticFiles / files/{name}
        // مهم → نباید Path.Combine استفاده شود
        session.VirtualFilePath =
            $"/files/{session.FileName}".Replace("\\", "/");

        session.Completed = true;

        await _repo.UpdateAsync(session, ct);
    }

    public Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken)
        => GetOwnedStatusAsync(uploadId, null, cancellationToken);

    public Task<UploadSession> GetStatusAsync(Guid uploadId, long? ownerUserId,
        CancellationToken cancellationToken = default)
        => GetOwnedStatusAsync(uploadId, ownerUserId, cancellationToken);

    private async Task<bool> HasExpectedSignatureAsync(string path, string fileName, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        var allowedExtensions = _fileSettings.Allowed3DMimeTypes
            .Concat(_fileSettings.AllowedImageMimeTypes)
            .Concat(_fileSettings.AllowedReadmeMimeTypes)
            .Select(x => x.ToLowerInvariant())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!allowedExtensions.Contains(extension))
            return false;

        var requiredBytes = extension switch
        {
            ".png" => new byte[] { 0x89, 0x50, 0x4E, 0x47 },
            ".jpg" or ".jpeg" => new byte[] { 0xFF, 0xD8, 0xFF },
            ".pdf" => new byte[] { 0x25, 0x50, 0x44, 0x46 },
            ".glb" => "glTF"u8.ToArray(),
            ".docx" => new byte[] { 0x50, 0x4B, 0x03, 0x04 },
            _ => null
        };

        if (requiredBytes is null)
            return extension is ".gltf" or ".json" or ".md" or ".txt";

        var buffer = new byte[requiredBytes.Length];
        await using var stream = File.OpenRead(path);
        var read = await stream.ReadAsync(buffer.AsMemory(), cancellationToken);
        return read == requiredBytes.Length && buffer.SequenceEqual(requiredBytes);
    }
    private async Task<UploadSession> GetOwnedStatusAsync(Guid uploadId, long? ownerUserId, CancellationToken cancellationToken)
    {
        var session = await _repo.GetAsync(uploadId, cancellationToken)
                      ?? throw new InvalidOperationException("Upload session not found.");
        EnsureOwner(session, ownerUserId);
        return session;
    }

    private static void EnsureOwner(UploadSession session, long? ownerUserId)
    {
        if (ownerUserId.HasValue && session.OwnerUserId != ownerUserId)
            throw new UnauthorizedAccessException("Upload session does not belong to the current user.");
    }
}
