namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Objects.Admin.Commands.CreateNewObject;
using Core.Contract.Features.Objects.Admin.Commands.Remove;
using Core.Contract.Features.Objects.Admin.Commands.Upgrade;
using Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;
using Core.Contract.Primitives.Models;
using Core.Contract.Primitives.Authorization;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

public sealed class ObjectController : AdminBaseController
{
    public ObjectController(IMediator mediator) : base(mediator)
    {
    }

    [Authorize(Policy = AdminPermission.ObjectWrite)]
    [HttpPatch("upgrade")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpgradeObject([FromBody] AdminUpgradeObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

 

    [HttpGet("by-id")]
    [Authorize(Policy = AdminPermission.ObjectRead)]
    [ProducesResponseType(typeof(Result<AdminObjectDetailsByIdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetAdminObjectDetailsByIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize(Policy = AdminPermission.ObjectWrite)]
    [HttpPost("add-new")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewObjectAsync([FromBody] AdminCreateNewObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
    
    
    
    [Authorize(Policy = AdminPermission.ObjectDelete)]
    [HttpDelete("remove")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveObjectAsync([FromBody] AdminRemoveObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
}
