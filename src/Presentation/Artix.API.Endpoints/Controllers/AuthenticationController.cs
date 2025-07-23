using Microsoft.AspNetCore.Mvc;

namespace Artix.API.Endpoints.Controllers;

using _primitives;
using Core.Contract.Features.Users.Commands.Modify;
using Core.Contract.Features.Users.Commands.RegisterAdmins;
using Core.Contract.Features.Users.Commands.RegisterMobiles;
using Core.Contract.Features.Users.Queries.GetUserProfile;
using Core.Contract.Features.Users.Queries.Login;
using Core.Contract.Features.Users.Queries.Logout;
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

    
    [HttpPost("register-mobile")]
    public async Task<IActionResult> RegisterMobileAsync(RegisterMobileCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }
    
    
    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAdminAdmin(RegisterAdminCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }
    
   
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(GetLoginQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }


    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> LogoutAsync(GetLogoutQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfileAsync([FromQuery] GetUserProfileQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }
   

 
    [Authorize]
    [HttpPatch("modify-profile")]
    public async Task<IActionResult> ModifyProfileAsync([FromBody] ModifyProfileCommand command)
    {
        var result = await this._mediator.Send(command);
        return Ok(result);
    }
}
