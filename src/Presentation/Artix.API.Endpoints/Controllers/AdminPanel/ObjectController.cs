namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Objects.Admin.Commands.CreateNewObject;
using Core.Contract.Features.Objects.Admin.Commands.Remove;
using Core.Contract.Features.Objects.Admin.Commands.Upgrade;
using Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GetObjectDetailsByIdQuery = Core.Contract.Features.Objects.Admin.Queries.GetObjectDetailsById.GetObjectDetailsByIdQuery;

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

 

    [HttpGet("by-id")]
    [ProducesResponseType(typeof(Result<ObjectDetailsByIdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetObjectDetailsByIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("add-new")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewObjectAsync([FromBody] CreateNewObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
    
    
    
    [Authorize]
    [HttpDelete("remove")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveObjectAsync([FromBody] RemoveObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
}
