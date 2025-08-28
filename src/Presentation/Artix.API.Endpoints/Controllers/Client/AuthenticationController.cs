namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Users.Commands.InitiateOTPAuth;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Queries.GetReNewAccessToken;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using Core.Contract.Features.Users.Queries.VerifyOTPAuth;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


public sealed class AuthenticationController : ClientBaseController
{
    public AuthenticationController(IMediator mediator) : base(mediator)
    {
    }


    [HttpPost("send-otp")]
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync(InitiateOTPAuthCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(BaseApiResponse<VerifyOTPAuthDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync([FromBody] GetVerifyOTPAuthQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }


   


    [HttpPost("renew-access-token")]
    [ProducesResponseType(typeof(BaseApiResponse<ReNewAccessTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] GetReNewAccessTokenQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(BaseApiResponse<LogoutDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAsync([FromBody] GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
