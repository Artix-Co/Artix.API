namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Museums.Queries.GetAll;
using Core.Contract.Features.Museums.Queries.GetById;
using Core.Contract.Features.Museums.Queries.GetMuseumJournalEntries;
using Core.Contract.Features.Museums.Queries.GetMuseumKeyStatus;
using Core.Contract.Features.Museums.Queries.GetMuseumObjects;
using Core.Contract.Features.Museums.Queries.GetObjects;
using Core.Contract.Features.Museums.Queries.GetObjectScans;
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
    [HttpGet("{museumId:long}")]
    public async Task<IActionResult> GetMuseumByIdAsync([FromRoute] long museumId)
    {
        var result = await _mediator.Send(new GetMuseumByIdQuery { Id = museumId });
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{museumId:long}/objects")]
    public async Task<IActionResult> GetMuseumObjectsAsync([FromRoute] long museumId)
    {
        var result = await _mediator.Send(new GetMuseumObjectsQuery { MuseumId = museumId });
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{museumId:long}/journal-entries")]
    public async Task<IActionResult> GetMuseumJournalEntriesAsync([FromRoute] long museumId)
    {
        var result = await _mediator.Send(new GetMuseumJournalEntriesQuery { MuseumId = museumId });
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{museumId:long}/key/status")]
    public async Task<IActionResult> GetMuseumKeyStatusAsync([FromRoute] long museumId)
    {
        var result = await _mediator.Send(new GetMuseumKeyStatusQuery { MuseumId = museumId });
        return Ok(result);
    }


    [HttpGet("objects")]
    public async Task<IActionResult> GetAllObjectsAsync([FromQuery] GetAllObjectsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }


    [HttpGet("objects/{objectId:long}/scan")]
    public async Task<IActionResult> GetObjectScanAsync([FromRoute] long objectId)
    {
        var result = await _mediator.Send(new GetObjectScanQuery { Id = objectId });
        return Ok(result);
    }
}
