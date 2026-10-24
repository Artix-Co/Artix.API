namespace Artix.API.Core.Contract.Primitives.Infra.File;

using Domain.Entities.File;

public interface IUploadService
{
    Task<UploadSession> InitiateAsync(string fileName, long totalSize, int chunkSize, long? ownerUserId = null,
        CancellationToken cancellationToken = default);
    Task MarkChunkReceivedAsync(Guid uploadId, int chunkIndex, long? ownerUserId = null,
        CancellationToken cancellationToken = default);
    Task MergeChunksAsync(Guid uploadId, long? ownerUserId = null, CancellationToken cancellationToken = default);
    Task<UploadSession> GetStatusAsync(Guid uploadId, CancellationToken cancellationToken);
    Task<UploadSession> GetStatusAsync(Guid uploadId, long? ownerUserId,
        CancellationToken cancellationToken = default);
}
