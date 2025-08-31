namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Users.Queries.GetAdminUserProfile;
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
    [ProducesResponseType(typeof(Result<AdminUserProfileDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfileAsync([FromQuery] GetAdminUserProfileQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
