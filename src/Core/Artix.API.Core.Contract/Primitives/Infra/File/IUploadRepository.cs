namespace Artix.API.Core.Contract.Primitives.Infra.File;

using Domain.Entities.File;

public interface IUploadRepository
{
    Task AddAsync(UploadSession session, CancellationToken cancellationToken);
    Task<UploadSession?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateAsync(UploadSession session, CancellationToken cancellationToken);
}
