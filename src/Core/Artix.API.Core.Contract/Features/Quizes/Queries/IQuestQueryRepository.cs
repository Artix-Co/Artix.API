namespace Artix.API.Core.Contract.Features.Quests.Queries;

using Domain.Entities.Quest;
using Domain.Entities.Quiz;
using GetShuffledQuests;
using Primitives.Repositories;
using Quizes.Queries.GetShuffledQuests;

public interface IQuestQueryRepository : IQueryRepository<Quiz>
{
    ValueTask<IEnumerable<ShuffledQuestsDto>> GetShuffledQuestsAsync(GetShuffledQuestsQuery dto,
        CancellationToken cancellationToken = default);
}
