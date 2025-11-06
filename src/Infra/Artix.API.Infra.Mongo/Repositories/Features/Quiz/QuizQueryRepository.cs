 
 

namespace Artix.API.Infra.Mongo.Repositories.Features.Quiz;

using Artix.API.Core.Domain.Entities.Quiz;
using Core.Contract.Features.Quizzes.Queries;
using Core.Contract.Features.Quizzes.Queries.GetShuffledQuizzes;
using Data.DbContext;
using Primitives;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public sealed class QuizQueryRepository : MongoQueryRepository<Quiz>, IQuestQueryRepository
{
    public QuizQueryRepository(MongoQueryContext queryDbContext, ILogger<MongoQueryRepository<Quiz>> logger)
        : base(queryDbContext, logger)
    {
    }

    public async ValueTask<IEnumerable<ShuffledQuizzesDto>> GetShuffledAsync(
        GetShuffledQuizzesQuery dto,
        CancellationToken cancellationToken = default)
    {
        this._logger.LogInformation("Fetching historical quizzes for shuffling...");

        var filter = Builders<Quiz>.Filter
            .Where(q => !q.IsDeleted && q.RelatedFeature == "HistoricalQuiz");

        var sort = Builders<Quiz>.Sort
            .Ascending(q => q.Priority)
            .Descending(q => q.CreatedAt);

        var quizzes = await this._queryDbContext.FindAsync(
            filter: filter,
            sort: sort,
            limit: dto.Count,
            cancellationToken: cancellationToken);

        var shuffled = quizzes
            .OrderBy(_ => Guid.NewGuid())
            .Select(q => new ShuffledQuizzesDto(
                Id: q.BusinessId,
                Title: q.Title,
                Description: q.Description,
                XpReward: q.XPReward,
                BonusXp: q.BonusXP,
                Tier: q.Tier,
                Priority: q.Priority,
                Deadline: q.Deadline,
                IsSeasonal: q.IsSeasonal,
                RequiredActions: q.RequiredActions.Select(a => new ShuffledQuizzesActionDto(
                    ActionType: a.ActionType,
                    Details: a.Details,
                    RequiredCount: a.RequiredCount)).ToList().AsReadOnly()))
            .ToList();

        this._logger.LogInformation("Successfully returned {Count} shuffled historical quizzes", shuffled.Count);
        return shuffled;
    }
}
