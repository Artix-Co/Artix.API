namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Museums.Admin.Commands.CreateNew;
using Core.Contract.Features.Museums.Admin.Commands.Remove;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;
using Core.Contract.Primitives.Models;
using Core.Contract.Primitives.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class MuseumController : AdminBaseController
{
    public MuseumController(IMediator mediator) : base(mediator)
    {
    }


    [HttpGet("all")]
    [Authorize(Policy = AdminPermission.MuseumRead)]
    [ProducesResponseType(typeof(Result<PaginatedResult<AdminPaginatedMuseumsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetAdminPaginateMuseumsQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }
    
    
    [HttpGet("objects")]
    [Authorize(Policy = AdminPermission.ObjectRead)]
    [ProducesResponseType(typeof(Result<PaginatedResult<AdminMuseumObjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromQuery] GetAdminMuseumObjectsQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }

    [Authorize(Policy = AdminPermission.MuseumWrite)]
    [HttpPost("add-new")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewMuseumAsync([FromBody] AdminCreateNewMuseumCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
    
    
    [Authorize(Policy = AdminPermission.MuseumDelete)]
    [HttpDelete("remove")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveMuseumAsync([FromBody] AdminRemoveMuseumCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
}
