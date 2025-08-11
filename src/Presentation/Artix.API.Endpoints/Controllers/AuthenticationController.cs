using Microsoft.AspNetCore.Mvc;

namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Users.Commands.InitiateOTPAuth;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Queries.GetAccessToken;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using Core.Contract.Features.Users.Queries.VerifyOTPAuth;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

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
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync(InitiateOTPAuthCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("verify-otp")]
    [ProducesResponseType(typeof(BaseApiResponse<VerifyOTPAuthDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterMobileAsync([FromBody] GetVerifyOTPAuthQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }


    [HttpPost("register-admin")]
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAdminAdmin([FromBody] RegisterAdminCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }


    [HttpPost("login")]
    [ProducesResponseType(typeof(BaseApiResponse<LoginDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] GetLoginQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }

    
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(BaseApiResponse<AccessTokenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] GetAccessTokenQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(typeof(BaseApiResponse<LogoutDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LogoutAsync([FromBody] GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }
}
