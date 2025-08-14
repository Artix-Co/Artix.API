namespace Artix.API.Infra.Sql.Repositories.Features.Versions;

using Core.Contract.Features.Versions.Queries;
using Core.Contract.Features.Versions.Queries.GetLast;
using Core.Domain.Entities.Version;
using Data.DbContexts;
using Exceptions;
using Microsoft.EntityFrameworkCore;
using Primitives;

public sealed class VersionQueryRepository : QueryRepository<AppVersion>, IVersionQueryRepository
{
    private readonly ArtixQueryDbContext _queryDbContext;

    public VersionQueryRepository(ArtixQueryDbContext queryDbContext) : base(queryDbContext)
    {
        this._queryDbContext = queryDbContext;
    }

    public async Task<LastVersionDto> GetLastAsync(
        GetLastVersionQuery dto,
        CancellationToken cancellationToken = default)
    {
        var result = new LastVersionDto();
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

        result.IsRequired = query.IsRequired;
        result.MinSupported = query.MinSupported;
        result.Description = query.Description;
        result.VersionString = query.VersionString;
        
        return result;
    }
}
