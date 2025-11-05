namespace Artix.API.Infra.Mongo.Data.Seed;

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.Entities.Quiz;
using DbContext;

 

public static class MongoDataSeeder
{
    public static async Task EnsureMongoMigrationAsync(IMongoDatabase database)
    {
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!collectionNames.Contains("Quizs"))
        {
            await database.CreateCollectionAsync("Quizs");
        }

        var collection = database.GetCollection<Quiz>("Quizs");
        var indexKeys = Builders<Quiz>.IndexKeys
            .Ascending(q => q.IsDeleted)
            .Ascending(q => q.RelatedFeature)
            .Ascending(q => q.Priority);

        var indexModel = new CreateIndexModel<Quiz>(indexKeys,
            new CreateIndexOptions { Name = "Quest_IsDeleted_RelatedFeature_Priority", Background = true });

        var existingIndexes = await (await collection.Indexes.ListAsync()).ToListAsync();
        if (existingIndexes.All(i => i["name"].AsString != "Quest_IsDeleted_RelatedFeature_Priority"))
        {
            await collection.Indexes.CreateOneAsync(indexModel);
        }
    }

    public static async Task SeedQuizzesAsync(MongoCommandContext commandContext)
    {
        var collection = commandContext.GetCollection<Quiz>("Quizs");

        await collection.DeleteManyAsync(Builders<Quiz>.Filter.Empty);

        var quizzes = GenerateSampleQuizzes(10);

        await commandContext.InsertManyAsync(quizzes);
    }

    private static List<Quiz> GenerateSampleQuizzes(int count)
    {
        var quizzes = new List<Quiz>();
        var random = new Random();
        var feature = "HistoricalQuiz";
        var questions = new[]
        {
            new { Title = "چه سالی انقلاب فرانسه رخ داد؟", Options = "A: 1789, B: 1812, C: 1917", Correct = "A", Description = "گزینه‌ها: A. 1789 B. 1812 C. 1917 - درست: A" },
            new { Title = "پایتخت امپراتوری عثمانی کجا بود؟", Options = "A: استانبول, B: قاهره, C: بغداد", Correct = "A", Description = "گزینه‌ها: A. استانبول B. قاهره C. بغداد - درست: A" },
            new { Title = "چه کسی دیوار چین را ساخت؟", Options = "A: امپراتور Qin Shi Huang, B: چنگیز خان, C: مارکو پولو", Correct = "A", Description = "گزینه‌ها: A. امپراتور Qin Shi Huang B. چنگیز خان C. مارکو پولو - درست: A" },
            new { Title = "جنگ جهانی اول در چه سالی آغاز شد؟", Options = "A: 1914, B: 1939, C: 1945", Correct = "A", Description = "گزینه‌ها: A. 1914 B. 1939 C. 1945 - درست: A" },
            new { Title = "کریستف کلمب چه قاره‌ای را کشف کرد؟", Options = "A: آمریکا, B: آسیا, C: آفریقا", Correct = "A", Description = "گزینه‌ها: A. آمریکا B. آسیا C. آفریقا - درست: A" },
            new { Title = "امپراتوری روم در چه قرنی سقوط کرد؟", Options = "A: قرن پنجم میلادی, B: قرن پانزدهم میلادی, C: قرن اول میلادی", Correct = "A", Description = "گزینه‌ها: A. قرن پنجم میلادی B. قرن پانزدهم میلادی C. قرن اول میلادی - درست: A" },
            new { Title = "چه کسی تئوری نسبیت را ارائه داد؟", Options = "A: آلبرت اینشتین, B: آیزاک نیوتن, C: گالیله", Correct = "A", Description = "گزینه‌ها: A. آلبرت اینشتین B. آیزاک نیوتن C. گالیله - درست: A" },
            new { Title = "رنسانس در کدام کشور آغاز شد؟", Options = "A: ایتالیا, B: فرانسه, C: انگلیس", Correct = "A", Description = "گزینه‌ها: A. ایتالیا B. فرانسه C. انگلیس - درست: A" },
            new { Title = "چه سالی انسان به ماه قدم گذاشت؟", Options = "A: 1969, B: 1957, C: 1975", Correct = "A", Description = "گزینه‌ها: A. 1969 B. 1957 C. 1975 - درست: A" },
            new { Title = "پادشاه مشهور مصر باستان کی بود؟", Options = "A: توت عنخ آمون, B: هانیبال, C: ژولیوس سزار", Correct = "A", Description = "گزینه‌ها: A. توت عنخ آمون B. هانیبال C. ژولیوس سزار - درست: A" }
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
        quiz.AddRequiredAction("AnswerQuestion", $"پاسخ درست: {correctOption} برای سوال {quiz.Title}", 1);
    }
}
