namespace Artix.API.Endpoints.Controllers.Common;

using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/shared/[controller]")]
public abstract class SharedController: ControllerBase
{
    protected readonly IMediator _mediator;

    protected SharedController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
