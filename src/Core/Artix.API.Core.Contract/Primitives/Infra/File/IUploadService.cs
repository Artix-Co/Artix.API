namespace Artix.API.Core.Contract.Primitives.Infra.File;

using Domain.Entities.File;

public interface IUploadService
{
    Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize,
        CancellationToken cancellationToken);

    Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex, CancellationToken cancellationToken);
    Task MergeChunksAsync(Guid uploadId, CancellationToken cancellationToken);
    Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken);
}
