namespace Artix.API.Core.ApplicationService.Features.Users.Queries.GetAccessToken;

using Contract.Features.Users.Queries.GetAccessToken;
using Domain.Entities.User;
using Infra.Identity.Interfaces.TokenService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetAccessTokenQueryHandler : QueryHandlerBase<GetAccessTokenQuery, AccessTokenDto>
{
    private readonly ITokenService _tokenService;

    public GetAccessTokenQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, ITokenService tokenService) : base(cache, httpContextAccessor, userManager)
    {
        this._tokenService = tokenService;
    }

    public override async Task<AccessTokenDto> Handle(GetAccessTokenQuery query, CancellationToken cancellationToken)
    {
        var tokenServiceResult =
            await this._tokenService.RefreshAccessTokenAsync(query.RefreshToken, cancellationToken);


        var result = new AccessTokenDto
        {
            AccessToken = tokenServiceResult.AccessToken,
            AccessTokenExpiresAt = tokenServiceResult.AccessTokenExpiresAt,
        };
        
        return result;
    }
}
