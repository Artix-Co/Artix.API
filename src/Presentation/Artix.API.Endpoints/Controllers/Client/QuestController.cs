namespace Artix.API.Endpoints.Controllers.Client;

using Common;
using Core.Contract.Features.Quests.Queries.GetShuffledQuests;
using Core.Contract.Features.Quizes.Queries.GetShuffledQuests;
using Core.Contract.Primitives.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

public class QuestController : ClientBaseController
{
    public QuestController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet("shuffled")]
    [ProducesResponseType(typeof(Result<IEnumerable<ShuffledQuestsDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetShuffledQuestsAsync([FromQuery] GetShuffledQuestsQuery query)
    {
        var result = await this._mediator.Send(query);
        return this.Ok(result);
    }
}
