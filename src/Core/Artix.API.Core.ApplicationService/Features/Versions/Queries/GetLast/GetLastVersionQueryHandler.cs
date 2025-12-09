namespace Artix.API.Core.ApplicationService.Features.Versions.Queries.GetLast;

using Primitives;
using Artix.API.Core.Contract.Features.Versions.Queries;
using Artix.API.Core.Contract.Features.Versions.Queries.GetLast;
using Domain.Entities.User;
using Contract.Primitives.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

internal sealed class
    GetLastVersionQueryHandler : QueryHandlerBase<GetLastVersionQuery,
    LastVersionDto>
{
    private readonly IVersionQueryRepository _versionQueryRepository;


    public GetLastVersionQueryHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager, IVersionQueryRepository versionQueryRepository) : base(httpContextAccessor, userManager)
    {
        this._versionQueryRepository = versionQueryRepository;
    }

    public override async Task<Result<LastVersionDto>> Handle(GetLastVersionQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._versionQueryRepository.GetLastAsync(query, cancellationToken);
        return Result<LastVersionDto>.Success(result);
    }
}
