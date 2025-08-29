namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

 
public sealed class MuseumController : ClientBaseController
{
    public MuseumController(IMediator mediator) : base(mediator) { }

    [HttpGet("all")]
    [ProducesResponseType(typeof(Result<IEnumerable<AllMuseumsClientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetAllMuseumsClientQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }

    [Authorize]
    [HttpGet("by-id")]
    [ProducesResponseType(typeof(Result<IEnumerable<AllMuseumsClientDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumByIdAsync([FromQuery] GetMuseumDetailsByIdQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    [Authorize]
    [HttpGet("objects")]
    [ProducesResponseType(typeof(Result<IEnumerable<MuseumObjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromQuery] GetMuseumObjectsQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }

    // [Authorize]
    // [HttpGet("journal-entries")]
    // [ProducesResponseType(typeof(Result<IEnumerable<MuseumJournalEntryDto>>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetMuseumJournalEntriesAsync([FromQuery] GetMuseumJournalEntriesQuery query)
    // {
    //     var result = await _mediator.Send(query);
    //     return Ok(result);
    // }
    //
    // [Authorize]
    // [HttpGet("key-status")]
    // [ProducesResponseType(typeof(Result<MuseumKeyStatusDto>), StatusCodes.Status200OK)]
    // public async Task<IActionResult> GetMuseumKeyStatusAsync([FromQuery] GetMuseumKeyStatusQuery query)
    // {
    //     var result = await _mediator.Send(query);
    //     return Ok(result);
    // }



    
    [Authorize]
    [HttpGet("recent")]
    [ProducesResponseType(typeof(Result<IEnumerable<UserRecentMuseumsVisitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentVisitedAsync([FromQuery] GetUserRecentMuseumsVisitQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
