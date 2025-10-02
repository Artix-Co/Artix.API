namespace Artix.API.Core.Contract.Features.Quests.Queries;

using Domain.Entities.Quest;
using Primitives.Repositories;

public interface IQuestQueryRepository: IQueryRepository<Quest>
{
    Task<long> GetTotalQuestsCountAsync();
    Task<List<Quest>> GetAllQuestsAsync();

}
