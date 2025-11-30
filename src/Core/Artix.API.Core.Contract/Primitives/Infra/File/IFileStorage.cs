namespace Artix.API.Core.Contract.Primitives.Infra.File;

public interface IFileStorage
{
    Task EnsureDirectoriesAsync(CancellationToken cancellationToken);
    Task SaveChunkAsync(Guid uploadId, int chunkIndex, Stream data, CancellationToken cancellationToken);

    Task MergeAsync(Guid uploadId, string fileName, int totalChunks, Stream outputStream,
        CancellationToken cancellationToken);

    Task<string> GetTempFolderAsync(Guid uploadId, CancellationToken cancellationToken);
    Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex, CancellationToken cancellationToken);
    Task<string> GetMergedFilePathAsync(Guid uploadId, string fileName, CancellationToken cancellationToken);
}
