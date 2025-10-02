namespace Artix.API.Core.Contract.Features.Quests.Commands;

using Domain.Entities.Quest;
using Primitives.Repositories;

public interface IQuestCommandRepository : ICommandRepository<Quest>
{
}
