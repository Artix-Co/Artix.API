namespace Artix.API.Core.Contract.Features.Quests.Queries.GetShuffledQuests;

using Primitives.Handlers;

public record GetShuffledQuestsQuery(int Count = 10) : IQuery<IEnumerable<ShuffledQuestsDto>>;
