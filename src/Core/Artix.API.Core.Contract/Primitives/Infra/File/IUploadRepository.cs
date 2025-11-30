
namespace Artix.API.Core.Contract.Primitives.Infra.File;

using Domain.Entities.File;

public interface IUploadRepository
{
    Task AddAsync(UploadSession session);
    Task<UploadSession> GetAsync(Guid id);
    Task UpdateAsync(UploadSession session);
}
