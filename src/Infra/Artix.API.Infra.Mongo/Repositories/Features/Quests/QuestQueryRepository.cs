namespace Artix.API.Infra.Mongo.Repositories.Features.Quests;

using Core.Contract.Features.Quests.Queries;
using Core.Domain.Entities.Quest;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using Primitives;

public sealed class QuestQueryRepository : MongoQueryRepository<Quest>, IQuestQueryRepository
{
    public QuestQueryRepository(MongoQueryContext queryDbContext, ILogger<MongoQueryRepository<Quest>> logger) : base(
        queryDbContext, logger)
    {
    }

    public async Task<long> GetTotalQuestsCountAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Counting total quests");
        var count = await this._queryDbContext.CountAsync<Quest>(cancellationToken);
        _logger.LogInformation("Total quests count: {Count}", count);
        return count;
    }

    public async Task<List<Quest>> GetAllQuestsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all quests");
        var quests = await this._queryDbContext.FindAsync<Quest>(cancellationToken);
        _logger.LogInformation("Retrieved {Count} quests", quests.Count);
        return quests;
    }
}
