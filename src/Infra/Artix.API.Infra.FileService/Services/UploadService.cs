namespace Artix.API.Infra.FileService.Services;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Core.Domain.Entities.File;
using Microsoft.Extensions.Options;
using Utils.File;

public class UploadService : IUploadService
{
    private readonly IUploadRepository _repo;
    private readonly IFileStorage _storage;
    private readonly FileSettings _fileSettings;

    public UploadService(IUploadRepository repo, IFileStorage storage, IOptions<FileSettings> fileSettings)
    {
        this._repo = repo;
        this._storage = storage;
        this._fileSettings = fileSettings.Value;
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
        
        var uniqueFileName = FileNameHelper.GenerateUniqueFileName(s.FileName);
        var finalPath = Path.Combine(_fileSettings.StoragePath, uniqueFileName);

        await _storage.MergeAsync(uploadId, s.FileName, s.TotalChunks, Stream.Null, cancellationToken);

        s.MergedFilePath = finalPath;
        s.FinalFileName = uniqueFileName; 
        s.Completed = true;

        await _repo.UpdateAsync(s, cancellationToken);
    }


    public Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken = default)
    {
        return this._repo.GetAsync(uploadId, cancellationToken);
    }
}
