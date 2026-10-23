namespace Artix.API.Endpoints.Controllers.Common;

using MediatR;
using Core.Contract.Primitives.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Authorize(Policy = AdminPermission.ClientAccess)]
[Route("api/client/[controller]")]
public abstract class ClientBaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected ClientBaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
