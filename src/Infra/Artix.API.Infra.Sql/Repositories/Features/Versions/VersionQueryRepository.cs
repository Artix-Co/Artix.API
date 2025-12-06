namespace Artix.API.Infra.Sql.Repositories.Features.Versions;

using Core.Contract.Features.Versions.Queries;
using Core.Contract.Features.Versions.Queries.GetLast;
using Core.Domain.Entities.Version;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class VersionQueryRepository : QueryRepository<AppVersion>, IVersionQueryRepository
{
    public VersionQueryRepository(ArtixQueryDbContext queryDbContext,
        ILogger<QueryRepository<AppVersion>> logger) : base(
        queryDbContext, logger)
    {
    }

    public async Task<LastVersionDto> GetLastAsync(
        GetLastVersionQuery dto,
        CancellationToken cancellationToken = default)
    {
        var query = await this._queryDbContext.AppVersions
            .Where(v => !v.IsDeleted)
            .OrderByDescending(v => v.Major)
            .ThenByDescending(v => v.Minor)
            .ThenByDescending(v => v.Patch)
            .FirstOrDefaultAsync(cancellationToken);


        if (query is null)
        {
            throw InfrastructureNotFoundException.WithMessage("No version found.");
        }

        var result = new LastVersionDto(query.IsRequired, query.MinSupported, query.Description, query.VersionString);
        return result;
    }
}
