using Microsoft.AspNetCore.Mvc;

namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Users.Commands.InitiateOTPAuth;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Queries.GetAccessToken;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using Core.Contract.Features.Users.Queries.VerifyOTPAuth;
using MediatR;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : BaseController
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator) : base(mediator)
    {
        this._mediator = mediator;
    }


    [HttpPost("send-otp")]
    public async Task<IActionResult> RegisterMobileAsync(InitiateOTPAuthCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> RegisterMobileAsync([FromBody] GetVerifyOTPAuthQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }


    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdminAdmin([FromBody] RegisterAdminCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] GetLoginQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }

    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> LoginAsync([FromBody] GetAccessTokenQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync([FromBody] GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }
}
