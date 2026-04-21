namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Museums.Admin.Commands.CreateNewMuseum;
using Core.Contract.Features.Museums.Admin.Commands.Remove;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateMuseums;
using Core.Contract.Features.Museums.Admin.Queries.GetPaginateObjects;
using Core.Contract.Primitives.Models;
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
    [ProducesResponseType(typeof(Result<PaginatedResult<PaginatedMuseumsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetPaginateMuseumsQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }
    
    
    [HttpGet("objects")]
    [ProducesResponseType(typeof(Result<PaginatedResult<AdminMuseumObjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromQuery] GetAdminMuseumObjectsQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("add-new")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddNewMuseumAsync([FromBody] CreateNewMuseumCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
    
    
    [Authorize]
    [HttpDelete("remove")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveMuseumAsync([FromBody] RemoveMuseumCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }
}
