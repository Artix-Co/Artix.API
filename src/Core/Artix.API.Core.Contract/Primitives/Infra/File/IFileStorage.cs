namespace Artix.API.Core.Contract.Primitives.Infra.File;

public interface IFileStorage
{
    Task EnsureDirectoriesAsync();
    Task SaveChunkAsync(Guid uploadId, int chunkIndex, Stream data, CancellationToken ct);
    Task MergeAsync(Guid uploadId, string fileName, int totalChunks, Stream outputStream, CancellationToken ct);
    Task<string> GetTempFolderAsync(Guid uploadId);
    Task<bool> ChunkExistsAsync(Guid uploadId, int chunkIndex);
}
