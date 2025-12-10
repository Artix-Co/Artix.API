namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Users.Client.Commands.InitiateOTPAuth;
using Core.Contract.Features.Users.Client.Queries.GetLogout;
using Core.Contract.Features.Users.Client.Queries.GetReNewAccessToken;
using Core.Contract.Features.Users.Client.Queries.GetVerifyOTPAuth;
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
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync(InitiateOTPAuthCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(Result<VerifyOTPAuthDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync([FromBody] GetVerifyOTPAuthQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }


   


    [HttpPost("renew-access-token")]
    [ProducesResponseType(typeof(Result<ReNewAccessTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] GetReNewAccessTokenQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(Result<LogoutDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAsync([FromBody] GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
