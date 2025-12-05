namespace Artix.API.Infra.Mongo.Data.Seed;

using System.Diagnostics;
using Core.Domain.Entities.Quiz;
using DbContext;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

public sealed class MongoDataSeeder
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly MongoCommandContext _context;
    private readonly ILogger<MongoDataSeeder> _logger;

    public MongoDataSeeder(IMongoDatabase mongoDatabase, MongoCommandContext context, ILogger<MongoDataSeeder> logger)
    {
        _mongoDatabase = mongoDatabase;
        _context = context;
        _logger = logger;
    }

    public async Task EnsureMongoMigrationAsync()
    {
        using var activity = new Activity("MongoDataSeeder.EnsureMigration").Start();
        _logger.LogInformation("MongoDataSeeder | Starting migration check for MongoDB database");

        try
        {
            var collectionNames = await (await _mongoDatabase.ListCollectionNamesAsync()).ToListAsync();
            _logger.LogDebug("MongoDataSeeder | Existing collections in database: {Collections}",
                string.Join(", ", collectionNames));

            if (!collectionNames.Contains("Quizs"))
            {
                _logger.LogInformation("MongoDataSeeder | Collection 'Quizs' not found → Creating new collection");
                await _mongoDatabase.CreateCollectionAsync("Quizs");
                _logger.LogInformation("MongoDataSeeder | Collection 'Quizs' created successfully");
            }
            else
            {
                _logger.LogDebug("MongoDataSeeder | Collection 'Quizs' already exists");
            }

            var collection = _mongoDatabase.GetCollection<Quiz>("Quizs");

            var indexKeys = Builders<Quiz>.IndexKeys
                .Ascending(q => q.IsDeleted)
                .Ascending(q => q.RelatedFeature)
                .Ascending(q => q.Priority);

            var indexModel = new CreateIndexModel<Quiz>(indexKeys,
                new CreateIndexOptions { Name = "Quest_IsDeleted_RelatedFeature_Priority", Background = true });

            var existingIndexes = await (await collection.Indexes.ListAsync()).ToListAsync();
            var indexNames = existingIndexes.Select(i => i["name"]?.ToString()).Where(n => n != null).ToList();

            if (indexNames.All(name => name != "Quest_IsDeleted_RelatedFeature_Priority"))
            {
                _logger.LogInformation(
                    "MongoDataSeeder | Compound index 'Quest_IsDeleted_RelatedFeature_Priority' does not exist → Creating index");
                await collection.Indexes.CreateOneAsync(indexModel);
                _logger.LogInformation("MongoDataSeeder | Compound index created successfully");
            }
            else
            {
                _logger.LogDebug(
                    "MongoDataSeeder | Compound index 'Quest_IsDeleted_RelatedFeature_Priority' already exists");
            }

            _logger.LogInformation("MongoDataSeeder | Migration check for Quizs collection completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDataSeeder | Error during migration application for Quizs collection");
            throw;
        }
    }

    public async Task SeedQuizzesAsync()
    {
        using var activity = new Activity("MongoDataSeeder.SeedQuizzes").Start();
        _logger.LogWarning(
            "MongoDataSeeder | Starting full wipe and reseed operation for Quizs collection (dev/staging only)");

        try
        {
            var collection = _context.GetCollection<Quiz>("Quizs");

            var deleteResult = await collection.DeleteManyAsync(Builders<Quiz>.Filter.Empty);
            _logger.LogInformation("MongoDataSeeder | All previous quizzes deleted | Deleted count: {DeletedCount}",
                deleteResult.DeletedCount);

            var quizzes = GenerateSampleQuizzes(10);
            _logger.LogInformation("MongoDataSeeder | Generated {Count} sample quizzes", quizzes.Count);

            await _context.InsertManyAsync(quizzes);
            _logger.LogInformation("MongoDataSeeder | Successfully inserted {Count} new quizzes into Quizs collection",
                quizzes.Count);

            _logger.LogWarning("MongoDataSeeder | Seeding operation for Quizs collection completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MongoDataSeeder | Error during seeding quizzes");
            throw;
        }
    }

    private static List<Quiz> GenerateSampleQuizzes(int count)
    {
        var quizzes = new List<Quiz>();
        var random = new Random();
        var feature = "HistoricalQuiz";
        var questions = new[]
        {
            new
            {
                Title = "چه سالی انقلاب فرانسه رخ داد؟",
                Options = "A: 1789, B: 1812, C: 1917",
                Correct = "A",
                Description = "گزینه‌ها: A. 1789 B. 1812 C. 1917 - درست: A"
            },
            new
            {
                Title = "پایتخت امپراتوری عثمانی کجا بود؟",
                Options = "A: استانبول, B: قاهره, C: بغداد",
                Correct = "A",
                Description = "گزینه‌ها: A. استانبول B. قاهره C. بغداد - درست: A"
            },
            new
            {
                Title = "چه کسی دیوار چین را ساخت؟",
                Options = "A: امپراتور Qin Shi Huang, B: چنگیز خان, C: مارکو پولو",
                Correct = "A",
                Description = "گزینه‌ها: A. امپراتور Qin Shi Huang B. چنگیز خان C. مارکو پولو - درست: A"
            },
            new
            {
                Title = "جنگ جهانی اول در چه سالی آغاز شد؟",
                Options = "A: 1914, B: 1939, C: 1945",
                Correct = "A",
                Description = "گزینه‌ها: A. 1914 B. 1939 C. 1945 - درست: A"
            },
            new
            {
                Title = "کریستف کلمب چه قاره‌ای را کشف کرد؟",
                Options = "A: آمریکا, B: آسیا, C: آفریقا",
                Correct = "A",
                Description = "گزینه‌ها: A. آمریکا B. آسیا C. آفریقا - درست: A"
            },
            new
            {
                Title = "امپراتوری روم در چه قرنی سقوط کرد؟",
                Options = "A: قرن پنجم میلادی, B: قرن پانزدهم میلادی, C: قرن اول میلادی",
                Correct = "A",
                Description = "گزینه‌ها: A. قرن پنجم میلادی B. قرن پانزدهم میلادی C. قرن اول میلادی - درست: A"
            },
            new
            {
                Title = "چه کسی تئوری نسبیت را ارائه داد؟",
                Options = "A: آلبرت اینشتین, B: آیزاک نیوتن, C: گالیله",
                Correct = "A",
                Description = "گزینه‌ها: A. آلبرت اینشتین B. آیزاک نیوتن C. گالیله - درست: A"
            },
            new
            {
                Title = "رنسانس در کدام کشور آغاز شد؟",
                Options = "A: ایتالیا, B: فرانسه, C: انگلیس",
                Correct = "A",
                Description = "گزینه‌ها: A. ایتالیا B. فرانسه C. انگلیس - درست: A"
            },
            new
            {
                Title = "چه سالی انسان به ماه قدم گذاشت؟",
                Options = "A: 1969, B: 1957, C: 1975",
                Correct = "A",
                Description = "گزینه‌ها: A. 1969 B. 1957 C. 1975 - درست: A"
            },
            new
            {
                Title = "پادشاه مشهور مصر باستان کی بود؟",
                Options = "A: توت عنخ آمون, B: هانیبال, C: ژولیوس سزار",
                Correct = "A",
                Description = "گزینه‌ها: A. توت عنخ آمون B. هانیبال C. ژولیوس سزار - درست: A"
            }
        };

        for (int i = 0; i < count; i++)
        {
            var question = questions[i];
            var xpReward = 100 + ((i + 1) * 50);
            var bonusXp = random.Next(25, 101);
            var tier = random.Next(1, 4);
            var priority = random.Next(5, 11);
            var deadline = random.Next(0, 2) == 0 ? DateTime.UtcNow.AddDays(random.Next(7, 31)) : (DateTime?)null;
            var isSeasonal = random.Next(0, 2) == 0;

            var quiz = Quiz.Create(
                title: question.Title,
                description: question.Description,
                xpReward: xpReward,
                bonusXp: bonusXp,
                tier: tier,
                priority: priority,
                deadline: deadline,
                isSeasonal: isSeasonal,
                relatedFeature: feature
            );

            AddActionsToQuiz(quiz, feature, random, question.Correct);
            quizzes.Add(quiz);
        }

        return quizzes;
    }

    private static void AddActionsToQuiz(Quiz quiz, string feature, Random random, string correctOption)
    {
        quiz.AddRequiredAction("AnswerQuestion", $"Correct answer: {correctOption} for question {quiz.Title}", 1);
    }
}
