namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Museums.Queries.GetAll;
using Core.Contract.Features.Museums.Queries.GetDetailByIds;
using Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/museums")]
public sealed class MuseumController : BaseController
{
    private readonly IMediator _mediator;

    public MuseumController(IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<AllMuseumDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetAllMuseumsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("by-id")]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<AllMuseumDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumByIdAsync([FromQuery] GetMuseumDetailsByIdQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("objects")]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<MuseumObjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromQuery] GetMuseumObjectsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("journal-entries")]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<MuseumJournalEntryDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumJournalEntriesAsync([FromQuery] GetMuseumJournalEntriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("key-status")]
    [ProducesResponseType(typeof(BaseApiResponse<MuseumKeyStatusDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMuseumKeyStatusAsync([FromQuery] GetMuseumKeyStatusQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }



    
    [Authorize]
    [HttpGet("recent")]
    [ProducesResponseType(typeof(BaseApiResponse<IEnumerable<UserRecentMuseumsVisitDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecentVisitedAsync([FromQuery] GetUserRecentMuseumsVisitQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
