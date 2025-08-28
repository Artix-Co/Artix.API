namespace Artix.API.Endpoints.Controllers.Common;

using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/[controller]")]
public abstract class AdminBaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected AdminBaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
