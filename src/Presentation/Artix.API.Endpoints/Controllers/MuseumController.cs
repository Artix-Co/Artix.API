namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Museums.Queries.GetAll;
using Core.Contract.Features.Museums.Queries.GetById;
using Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Museums.Queries.GetUserRecentMuseumsVisits;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetAllMuseumsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("by-id")]
    public async Task<IActionResult> GetMuseumByIdAsync([FromQuery] GetMuseumByIdQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("objects")]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromQuery] GetMuseumObjectsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("journal-entries")]
    public async Task<IActionResult> GetMuseumJournalEntriesAsync([FromQuery] GetMuseumJournalEntriesQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("/key/status")]
    public async Task<IActionResult> GetMuseumKeyStatusAsync([FromRoute] long museumId)
    {
        var result = await _mediator.Send(new GetMuseumKeyStatusQuery { MuseumId = museumId });
        return Ok(result);
    }



    
    [Authorize]
    [HttpGet("recent")]
    public async Task<IActionResult> GetAllObjectsAsync([FromQuery] GetUserRecentMuseumsVisitQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
