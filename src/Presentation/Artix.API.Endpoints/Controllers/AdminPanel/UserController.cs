namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Users.Admin.Queries.GetPaginateUsers;
using Core.Contract.Features.Users.Admin.Queries.GetUserProfile;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class UserController : AdminBaseController
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
    [HttpGet("all")]
    [ProducesResponseType(typeof(Result<PaginatedResult<PaginateUsersDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllUsersAsync([FromQuery] GetPaginateUsersQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
