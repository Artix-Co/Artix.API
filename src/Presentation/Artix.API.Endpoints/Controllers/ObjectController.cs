namespace Artix.API.Endpoints.Controllers;

using Common;
using Core.Contract.Features.Objects.Commands.ScanObject;
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
}
