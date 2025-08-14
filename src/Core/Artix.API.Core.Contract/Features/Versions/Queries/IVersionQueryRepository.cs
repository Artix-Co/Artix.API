namespace Artix.API.Core.Contract.Features.Versions.Queries;

using Domain.Entities.Version;
using GetLast;
using Primitives.Repositories;

public interface IVersionQueryRepository : IQueryRepository<AppVersion>
{
    Task<LastVersionDto> GetLastAsync(
        GetLastVersionQuery query, CancellationToken cancellationToken = default);
}
