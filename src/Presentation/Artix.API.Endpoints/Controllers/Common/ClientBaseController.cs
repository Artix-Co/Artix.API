namespace Artix.API.Endpoints.Controllers.Common;

using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/client/[controller]")]
public abstract class ClientBaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected ClientBaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
