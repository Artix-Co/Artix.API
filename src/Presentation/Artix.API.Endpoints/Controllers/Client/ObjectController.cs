namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Objects.Commands.AddToUserCollection;
using Core.Contract.Features.Objects.Commands.Scan;
using Core.Contract.Features.Objects.Commands.Upgrade;
using Core.Contract.Features.Objects.Queries.GetDetailByIds;
using Core.Contract.Features.Objects.Queries.GetUserRecentObjectsVisits;
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
    [ProducesResponseType(typeof(BaseApiResponse<PagedData<AllObjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllObjectsAsync([FromQuery] GetAllObjectsQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("scan")]
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ScanObject([FromBody] ScanObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [Authorize]
    [HttpGet("by-id")]
    [ProducesResponseType(typeof(BaseApiResponse<ObjectDetailByIdDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetObject([FromQuery] GetObjectDetailByIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPatch("upgrade")]
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpgradeObject([FromBody] UpgradeObjectCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }

    [Authorize]
    [HttpPost("add-to-collection")]
    [ProducesResponseType(typeof(BaseApiResponse<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddObjectToUserCollection([FromBody] AddObjectToUserCollectionCommand command)
    {
        var result = await this._mediator.Send(command);
        return this.Ok(result);
    }


    [Authorize]
    [HttpGet("recent")]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<UserRecentObjectsVisitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentVisitedAsync([FromQuery] GetUserRecentObjectsVisitQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
