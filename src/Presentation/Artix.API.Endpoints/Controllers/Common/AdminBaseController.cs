namespace Artix.API.Endpoints.Controllers.Common;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Artix.API.Core.Contract.Primitives.Authorization;

[ApiController]
[Authorize(Policy = AdminPermission.PortalRead)]
[Route("api/admin/[controller]")]
public abstract class AdminBaseController : ControllerBase
{
    protected readonly IMediator _mediator;

    protected AdminBaseController(IMediator mediator)
    {
        _mediator = mediator;
    }
}
