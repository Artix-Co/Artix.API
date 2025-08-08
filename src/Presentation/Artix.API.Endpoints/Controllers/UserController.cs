namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Collections.Queries.GetCollectionByUserId;
using Core.Contract.Features.Collections.Queries.GetUserCollection;
using Core.Contract.Features.Users.Commands.Modify;
using Core.Contract.Features.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/user")]
public sealed class UserController : BaseController
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator) : base(mediator)
    {
        this._mediator = mediator;
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


    [Authorize]
    [HttpGet("collection")]
    public async Task<IActionResult> CollectionsAsync([FromQuery] GetCollectionsByUserIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return Ok(result);
    }
}
