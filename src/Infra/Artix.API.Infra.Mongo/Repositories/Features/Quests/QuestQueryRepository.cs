namespace Artix.API.Infra.Mongo.Repositories.Features.Quests;

using Core.Contract.Features.Quests.Queries;
using Core.Domain.Entities.Quest;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Primitives;

public sealed class QuestQueryRepository : MongoQueryRepository<Quest>, IQuestQueryRepository
{
    public QuestQueryRepository(IMongoDatabase database, ILogger<MongoQueryRepository<Quest>> logger) : base(database,
        logger)
    {
    }

    public async Task<long> GetTotalQuestsCountAsync()
    {
        _logger.LogInformation("Counting total quests");
        var filter = Builders<Quest>.Filter.Eq("IsDeleted", false); // فقط Questهای غیرحذف‌شده
        var count = await _collection.CountDocumentsAsync(filter);
        _logger.LogInformation("Total quests count: {Count}", count);
        return count;
    }

    public async Task<List<Quest>> GetAllQuestsAsync()
    {
        _logger.LogInformation("Retrieving all quests");
        var filter = Builders<Quest>.Filter.Eq("IsDeleted", false); // فقط Questهای غیرحذف‌شده
        var quests = await _collection.Find(filter).ToListAsync();
        _logger.LogInformation("Retrieved {Count} quests", quests.Count);
        return quests;
    }
}
