namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetReNewAccessToken;

using Primitives;
using Artix.API.Core.Contract.Features.Users.Queries.GetReNewAccessToken;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Infra.Identity.Interfaces.TokenService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

// TODO: develop validator for this handler
internal sealed class GetReNewAccessTokenQueryHandler : QueryHandlerBase<GetReNewAccessTokenQuery, ReNewAccessTokenDto>
{
    private readonly ITokenService _tokenService;

    public GetReNewAccessTokenQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ITokenService tokenService) : base(cache, httpContextAccessor, userManager)
    {
        this._tokenService = tokenService;
    }

    public override async Task<Result<ReNewAccessTokenDto>> Handle(GetReNewAccessTokenQuery query,
        CancellationToken cancellationToken)
    {
        var tokenServiceResult = await this._tokenService.ReNewAccessTokenAsync(query.RefreshToken, cancellationToken);
        var result = new ReNewAccessTokenDto(tokenServiceResult.AccessToken, tokenServiceResult.AccessTokenExpiresAt);
        return Result<ReNewAccessTokenDto>.Success(result);
    }
}
