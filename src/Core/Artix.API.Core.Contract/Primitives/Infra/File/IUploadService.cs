namespace Artix.API.Core.Contract.Primitives.Infra.File;

using Domain.Entities.File;

public interface IUploadService
{
    Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize);
    Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex);
    Task MergeChunksAsync(Guid uploadId);
    Task<UploadSession> GetStatusAsync(Guid uploadId);
}
