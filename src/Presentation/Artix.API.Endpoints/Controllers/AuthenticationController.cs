using Microsoft.AspNetCore.Mvc;

namespace Artix.API.Endpoints.Controllers;

using System.Security.Claims;
using _primitives;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;

[ApiController]
[Route("api/auth")]
public sealed class AuthenticationController : BaseController
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator) : base(mediator)
    {
        this._mediator = mediator;
    }

   
    [HttpPost("login")]
    public async Task<IActionResult> Login(GetLoginQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }

    // [HttpPost("register")]
    // [HttpGet("profile")]
    // [HttpPatch("profile")]
}
