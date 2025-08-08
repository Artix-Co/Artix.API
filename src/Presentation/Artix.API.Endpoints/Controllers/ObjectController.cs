namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Objects.Commands.AddToUserCollection;
using Core.Contract.Features.Objects.Commands.Scan;
using Core.Contract.Features.Objects.Commands.Upgrade;
using Core.Contract.Features.Objects.Queries.GetDetailByIds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/objects")]
public sealed class ObjectController : BaseController
{
    private readonly IMediator _mediator;

    public ObjectController(IMediator mediator) : base(mediator)
    {
        _mediator = mediator;
    }
    
    [Authorize]
    [HttpPost("scan")]
    public async Task<IActionResult> ScanObject([FromBody] ScanObjectCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetObject(long id)
    {
        var query = new GetObjectDetailByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPatch("upgrade")]
    public async Task<IActionResult> UpgradeObject([FromBody] UpgradeObjectCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPost("add-to-collection")]
    public async Task<IActionResult> AddObjectToUserCollection([FromBody] AddObjectToUserCollectionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
