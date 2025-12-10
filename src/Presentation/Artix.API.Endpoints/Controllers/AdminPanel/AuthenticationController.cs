namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Users.Admin.Commands.Register;
using Core.Contract.Features.Users.Admin.Queries.GetLogin;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class AuthenticationController: AdminBaseController
{
    public AuthenticationController(IMediator mediator) : base(mediator)
    {
    }

    
    [HttpPost("register")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RegisterAdminAdmin([FromBody] RegisterCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }


    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<LoginDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> LoginAsync([FromBody] GetLoginQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
