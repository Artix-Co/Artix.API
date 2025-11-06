namespace Artix.API.Core.ApplicationService.Features.Quests.Queries.GetShuffledQuests;

using Contract.Features.Quizzes.Queries;
using Contract.Features.Quizzes.Queries.GetShuffledQuizzes;
using Contract.Primitives.Models;
using Domain.Entities.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Primitives;

internal sealed class
    GetShuffledQuestsQueryHandler : QueryHandlerBase<GetShuffledQuizzesQuery, IEnumerable<ShuffledQuizzesDto>>
{
    private readonly IQuestQueryRepository _questQueryRepository;

    public GetShuffledQuestsQueryHandler(IMemoryCache cache, IHttpContextAccessor httpContextAccessor,
        UserManager<AppUser> userManager, IQuestQueryRepository questQueryRepository) : base(cache, httpContextAccessor,
        userManager)
    {
        this._questQueryRepository = questQueryRepository;
    }

    public override async Task<Result<IEnumerable<ShuffledQuizzesDto>>> Handle(GetShuffledQuizzesQuery query,
        CancellationToken cancellationToken)
    {
        var result = await this._questQueryRepository.GetShuffledAsync(query, cancellationToken);
        return Result<IEnumerable<ShuffledQuizzesDto>>.Success(result);
    }
}
