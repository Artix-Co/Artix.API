namespace Artix.API.Core.Contract.Features.Quests.Queries.GetShuffledQuests;

using Primitives.Handlers;
using Quizes.Queries.GetShuffledQuests;

public record GetShuffledQuestsQuery(int Count = 10) : IQuery<IEnumerable<ShuffledQuestsDto>>;
