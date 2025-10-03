namespace Artix.API.Core.Contract.Features.Quests.Queries;

using Domain.Entities.Quest;
using GetShuffledQuests;
using Primitives.Repositories;

public interface IQuestQueryRepository : IQueryRepository<Quest>
{
    ValueTask<IEnumerable<ShuffledQuestsDto>> GetShuffledQuestsAsync(GetShuffledQuestsQuery dto,
        CancellationToken cancellationToken = default);
}
