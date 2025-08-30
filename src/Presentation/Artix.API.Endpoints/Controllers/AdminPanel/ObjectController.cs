namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Objects.Queries.GetAllObjectsAdmins;
using Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdAdmins;
using Core.Contract.Features.Objects.Queries.GetObjectDetailsByIdClients;
using Core.Contract.Primitives.Models;
using MediatR;
 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Core.Contract.Features.Objects.Commands.Upgrade;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class ObjectController : AdminBaseController
{
    public ObjectController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize]
    [HttpPatch("upgrade")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpgradeObject([FromBody] UpgradeObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(Result<PaginationQuery<AllObjectsAdminDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetAllObjectsAdminQuery adminQuery)
    {
        var result = await this._mediator.Send(adminQuery);
        return this.Ok(result);
    }

    [HttpGet("by-id")]
    [ProducesResponseType(typeof(Result<ObjectDetailsByIdAdminDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetObjectDetailsByIdAdminQuery adminQuery)
    {
        var result = await this._mediator.Send(adminQuery);
        return this.Ok(result);
    }
}
