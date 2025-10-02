namespace Artix.API.Core.Contract.Features.Quests.Queries;

using Domain.Entities.Quest;
using Primitives.Repositories;

public interface IQuestQueryRepository: IQueryRepository<Quest>
{
    Task<long> GetTotalQuestsCountAsync(CancellationToken cancellationToken = default);
    Task<List<Quest>> GetAllQuestsAsync(CancellationToken cancellationToken = default);

}
