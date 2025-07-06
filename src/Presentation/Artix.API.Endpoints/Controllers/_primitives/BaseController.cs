namespace Artix.API.Endpoints.Controllers._primitives;

using MediatR;
using Microsoft.AspNetCore.Mvc;

public abstract class BaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected BaseController(IMediator mediator)
    {
        this._mediator = mediator;
    }
}
