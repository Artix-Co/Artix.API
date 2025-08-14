namespace Artix.API.Core.ApplicationService.Features.Versions.Queries.GetLast;

using Artix.API.Core.ApplicationService.Primitives;
using Artix.API.Core.Contract.Features.Versions.Commands;
using Artix.API.Core.Contract.Features.Versions.Queries;
using Artix.API.Core.Contract.Features.Versions.Queries.GetLast;
using Artix.API.Core.Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

internal sealed class
    GetLastVersionQueryHandler : QueryHandlerBase<GetLastVersionQuery,
    LastVersionDto>
{
    private readonly IVersionQueryRepository _versionQueryRepository;

    public GetLastVersionQueryHandler(
        IMemoryCache cache,
        IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager,
        IVersionQueryRepository versionQueryRepository
    ) : base(cache, httpContextAccessor, userManager)
    {
        this._versionQueryRepository = versionQueryRepository;
    }

    public override async Task<LastVersionDto> Handle(GetLastVersionQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._versionQueryRepository.GetLastAsync(query, cancellationToken);
        return result;
    }
}
