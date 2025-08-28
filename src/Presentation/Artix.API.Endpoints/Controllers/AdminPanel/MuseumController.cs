namespace Artix.API.Endpoints.Controllers.AdminPanel;

using Common;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsAdmin;
using Core.Contract.Features.Museums.Queries.GetAllMuseumsClient;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class MuseumController:AdminBaseController
{
    public MuseumController(IMediator mediator) : base(mediator)
    {
    }
    
    
    [HttpGet("all")]
    [ProducesResponseType(typeof(Result<PaginatedResult<AllMuseumsAdminDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMuseumsAsync([FromQuery] GetAllMuseumsAdminQuery clientQuery)
    {
        var result = await this._mediator.Send(clientQuery);
        return this.Ok(result);
    }
}
