namespace Artix.API.Core.ApplicationService.Features.Quests.Queries.GetShuffledQuests;

using Contract.Features.Quests.Queries;
using Contract.Features.Quests.Queries.GetShuffledQuests;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetShuffledQuestsQueryHandler : QueryHandlerBase<GetShuffledQuestsQuery, IEnumerable<ShuffledQuestsDto>>
{
    private readonly IQuestQueryRepository _questQueryRepository;

    public GetShuffledQuestsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IQuestQueryRepository questQueryRepository) : base(cache, httpContextAccessor,
        userManager)
    {
        this._questQueryRepository = questQueryRepository;
    }

    public override async Task<Result<IEnumerable<ShuffledQuestsDto>>> Handle(GetShuffledQuestsQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._questQueryRepository.GetShuffledQuestsAsync(query, cancellationToken);
        return Result<IEnumerable<ShuffledQuestsDto>>.Success(result);
    }
}
