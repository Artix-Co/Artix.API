namespace Artix.API.Infra.Mongo.Repositories.Features.Quiz;

using Core.Contract.Features.Quests.Queries;
using Core.Contract.Features.Quests.Queries.GetShuffledQuests;
using Core.Contract.Features.Quizes.Queries.GetShuffledQuests;
using Core.Domain.Entities.Quiz;
using Data.DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Primitives;
using Utils.List;

public sealed class QuizQueryRepository : MongoQueryRepository<Quiz>, IQuestQueryRepository
{
    private static readonly Random _random = new();

    public QuizQueryRepository(MongoQueryContext queryDbContext, ILogger<MongoQueryRepository<Quiz>> logger)
        : base(queryDbContext, logger)
    {
    }


    public async ValueTask<IEnumerable<ShuffledQuestsDto>> GetShuffledQuestsAsync(
        GetShuffledQuestsQuery dto,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching historical quizzes for shuffling...");

        var filter = Builders<Quiz>.Filter.Where(q => !q.IsDeleted && q.RelatedFeature == "HistoricalQuiz");
        var sort = Builders<Quiz>.Sort.Ascending(q => q.Priority).Descending(q => q.CreatedAt);

        var quizzes = await _queryDbContext.FindAsync(
            filter: filter,
            sort: sort,
            limit: dto.Count,
            cancellationToken: cancellationToken);

        var shuffled = new List<ShuffledQuestsDto>(quizzes.Count);

        foreach (var quiz in quizzes)
        {
            // var descriptionParts = quiz.Description.Split(" - درست: ");
            // var optionsText = descriptionParts[0]
            //     .Replace("گزینه‌ها: ", "")
            //     .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries)
            //     .Where(o => o.Length == 1 && (o[0] == 'A' || o[0] == 'B' || o[0] == 'C'))
            //     .ToArray();
            //
            // var correctAnswer = descriptionParts[1].Trim();
            // var originalIndex = Array.IndexOf(optionsText, correctAnswer);
            // if (originalIndex == -1) continue;
            //
            // var shuffledOptions = optionsText.OrderBy(_ => _random.Next()).ToArray();
            // var correctId = (byte)Array.IndexOf(shuffledOptions, correctAnswer);
            //
            // shuffled.Add(new ShuffledQuestsDto(
            //     quiz.BusinessId,
            //     quiz.Title,
            //     shuffledOptions,
            //     correctId,
            //     quiz.CreatedAt));
        }

        _logger.LogInformation("Successfully returned {Count} shuffled historical quizzes", shuffled.Count);
        return shuffled;
    }
}
