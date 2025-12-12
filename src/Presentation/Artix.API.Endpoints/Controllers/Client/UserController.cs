namespace Artix.API.Endpoints.Controllers.Client;

using Core.Contract.Primitives.Models;
using Common;
using Core.Contract.Features.Users.Client.Commands.DeleteProfile;
using Core.Contract.Features.Users.Client.Commands.ModifyProfile;
using Core.Contract.Features.Users.Client.Queries.GetPaginateLoginHistories;
using Core.Contract.Features.Users.Client.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class UserController : ClientBaseController
{
    public UserController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpGet("profile")]
    [ProducesResponseType(typeof(Result<UserProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileAsync([FromQuery] GetUserProfileQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }


    [Authorize]
    [HttpPatch("modify-profile")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ModifyProfileAsync([FromBody] ModifyProfileCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
    
    
    [Authorize]
    [HttpDelete("delete-profile")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeActivateProfileAsync([FromQuery] DeleteProfileCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [Authorize]
    [HttpGet("login-history")]
    [ProducesResponseType(typeof(Result<PaginatedResult<PaginateLoginHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileAsync([FromQuery] GetPaginateLoginHistoryQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    // [Authorize]
    // [HttpGet("collection")]
    // [ProducesResponseType(typeof(Result<IEnumerable<CollectionsByUserIdDto>>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> CollectionsAsync([FromQuery] GetCollectionsByUserIdQuery query)
    // {
    //     var result = await this._mediator.Send(query);
    //     return Ok(result);
    // }
}
