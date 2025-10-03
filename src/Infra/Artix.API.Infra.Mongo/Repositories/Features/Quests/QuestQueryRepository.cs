namespace Artix.API.Infra.Mongo.Repositories.Features.Quests;

using Core.Contract.Features.Quests.Queries;
using Core.Contract.Features.Quests.Queries.GetShuffledQuests;
using Core.Domain.Entities.Quest;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Primitives;

public sealed class QuestQueryRepository : MongoQueryRepository<Quest>, IQuestQueryRepository
{
    private static readonly Random _random = Random.Shared;

    public QuestQueryRepository(MongoQueryContext queryDbContext, ILogger<MongoQueryRepository<Quest>> logger)
        : base(queryDbContext, logger)
    {
    }

    public async ValueTask<IEnumerable<ShuffledQuestsDto>> GetShuffledQuestsAsync(
        GetShuffledQuestsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching and shuffling quests...");

        // Define filter and sort for the query
        var filter = Builders<Quest>.Filter.Empty; // Empty filter to get all non-deleted quests
        var sort = Builders<Quest>.Sort.Descending(q => q.CreatedAt);

        // Fetch quests using MongoQueryContext
        var quests = await _queryDbContext.FindAsync(
            filter: filter,
            sort: sort,
            limit: dto.Count,
            cancellationToken: cancellationToken);

        // Prepare result list
        int maxCount = dto.Count;
        var result = new List<ShuffledQuestsDto>();

        foreach (var quest in quests)
        {
            if (result.Count >= maxCount) break;
            if (quest.RequiredActions.Count == 0) continue;

            // Get up to 4 options
            int optionCount = Math.Min(quest.RequiredActions.Count, 4);
            var optionsArray = new string[optionCount];
            byte correctOptionId = 0;

            // Fill options and find correct option
            for (byte i = 0; i < optionCount; i++)
            {
                optionsArray[i] = quest.RequiredActions[i].Details ?? "";
                if (quest.RequiredActions[i].RequiredCount > 0 && correctOptionId == 0)
                {
                    correctOptionId = i;
                }
            }

            // Shuffle options
            Shuffle(optionsArray);

            // Find new correct option ID after shuffle
            byte newCorrectOptionId = 0;
            for (byte i = 0; i < optionsArray.Length; i++)
            {
                if (optionsArray[i] != quest.RequiredActions[correctOptionId].Details) continue;
                newCorrectOptionId = i;
                break;
            }

            // Add to result
            result.Add(new ShuffledQuestsDto(
                quest.BusinessId,
                quest.Title,
                optionsArray.AsMemory(),
                newCorrectOptionId,
                quest.CreatedAt));
        }

        // Shuffle quests
        Shuffle(result);

        _logger.LogInformation("Fetched and shuffled {Count} quests", result.Count);
        return result;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    private static void Shuffle<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = _random.Next(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }
}
