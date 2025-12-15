namespace Artix.API.Core.ApplicationService.Features.Users.Client.Queries.GetVerifyOTPAuth;

using Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
using Contract.Primitives.Infra.Identity.Authentication;
using Contract.Primitives.Infra.Identity.Authentication.Client.Login;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Primitives;

// TODO: develop validator for this handler
internal sealed class GetVerifyOTPAuthHandler : QueryHandlerBase<GetVerifyOTPAuthQuery, VerifyOTPAuthDto>
{
    private readonly IAuthenticationService _authenticationService;

    public GetVerifyOTPAuthHandler(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager,
        IAuthenticationService authenticationService) : base(httpContextAccessor, userManager)
    {
        this._authenticationService = authenticationService;
    }

    public override async Task<Result<VerifyOTPAuthDto>> Handle(GetVerifyOTPAuthQuery query,
        CancellationToken cancellationToken)
    {
        var authenticationResult =
            await this._authenticationService.ClientOtpLoginAsync(
                new ClientLoginRequest(query.PhoneNumber, query.OtpCode), cancellationToken);

        var result = new VerifyOTPAuthDto(
            UserId: authenticationResult.UserId,
            AccessToken: authenticationResult.AccessToken,
            RefreshToken: authenticationResult.RefreshToken,
            AccessTokenExpiresAt: authenticationResult.AccessTokenExpiresAt,
            RefreshTokenExpiresAt: authenticationResult.RefreshTokenExpiresAt);
        
        
        return Result<VerifyOTPAuthDto>.Success(result);
    }
}
