namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Objects.Client.Commands.AddToUserCollection;
using Core.Contract.Features.Objects.Client.Commands.Scan;
using Core.Contract.Features.Objects.Client.Queries.GetObjectDetailsById;
using Core.Contract.Features.Objects.Client.Queries.GetPaginateObjects;
using Core.Contract.Features.Objects.Client.Queries.GetUserRecentObjectsVisits;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class ObjectController : ClientBaseController
{
    public ObjectController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(Result<PaginatedResult<ClientPaginateObjectsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllObjectsAsync([FromQuery] GetClientPaginateObjectsQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("scan")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ScanObject([FromBody] ClientScanObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [Authorize]
    [HttpGet("by-id")]
    [ProducesResponseType(typeof(Result<ClientObjectDetailsByIdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetClientObjectDetailsByIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

  

    [Authorize]
    [HttpPost("add-to-collection")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddObjectToUserCollection([FromBody] AddCleintObjectToUserCollectionCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }


    [Authorize]
    [HttpGet("recent")]
    [ProducesResponseType(typeof(Result<IEnumerable<ClientUserRecentObjectsVisitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentVisitedAsync([FromQuery] GetClientUserRecentObjectsVisitQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
