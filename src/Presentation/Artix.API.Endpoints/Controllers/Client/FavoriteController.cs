namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Favorites.Client.Commands.SetFavorite;
using Core.Contract.Features.Favorites.Client.Queries.GetFavorites;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public sealed class FavoriteController : ClientBaseController
{
    public FavoriteController(IMediator mediator) : base(mediator) { }

    [Authorize]
    [HttpPut("set")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SetAsync([FromBody] SetFavoriteCommand command)
        => Ok(await _mediator.Send(command));

    [Authorize]
    [HttpGet]
    [ProducesResponseType(typeof(Result<IReadOnlyList<FavoriteDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync([FromQuery] GetFavoritesQuery query)
        => Ok(await _mediator.Send(query));
}
