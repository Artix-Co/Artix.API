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
        // چک کردن وجود مجموعه quiz
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!collectionNames.Contains("Quizzes"))
        {
            await database.CreateCollectionAsync("Quizzes");
        }

        // ایجاد ایندکس‌ها
        var collection = database.GetCollection<Quiz>("Quizzes");
        var indexKeys = Builders<Quiz>.IndexKeys
            .Ascending("IsDeleted")
            .Ascending("RelatedFeature")
            .Ascending("Priority");

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
        var collection =
            commandContext.GetCollection<Quiz>("Quizzes"); // Note: Changed to plural "quizs" to match MongoQueryContext convention

        // Clear collection to avoid duplicate key errors
        await collection.DeleteManyAsync(Builders<Quiz>.Filter.Empty);


        // Generate 10 sample quizs
        var quizs = GenerateSampleQuizzes(10);

        // Insert quizs into MongoDB
        await commandContext.InsertManyAsync(quizs);
    }

    private static List<Quiz> GenerateSampleQuizzes(int count)
    {
        var quizzes = new List<Quiz>();
        var random = new Random();
        var features = new[] { "QRHunts", "LastQuiz", "Strike", "TreasureHunt", "DailyChallenge" };
        var titles = new[]
        {
            "اسکن QR در موزه {0}", "تکمیل کوئیز {0}", "حفظ Strike برای {0} روز", "شکار گنج در {0}",
            "چالش روزانه {0}", "اکتشاف آثار باستانی {0}", "ماموریت ویژه {0}", "جمع‌آوری امتیاز در {0}",
            "مسابقه سرعت {0}", "کاوش در تاریخ {0}"
        };
        var locations = new[] { "تهران", "شیراز", "اصفهان", "مشهد", "تبریز" };

        for (int i = 1; i <= count; i++)
        {
            var feature = features[random.Next(features.Length)];
            var location = locations[random.Next(locations.Length)];
            var title = string.Format(titles[i - 1], location);
            var description = GenerateDescription(feature, location, random);
            var xpReward = 100 + (i * 50); // 150, 200, 250, ...
            var bonusXp = random.Next(25, 101); // Random bonus between 25 and 100
            var tier = random.Next(1, 4); // Random tier between 1 and 3
            var priority = random.Next(5, 11); // Random priority between 5 and 10
            var deadline = random.Next(0, 2) == 0 ? DateTime.UtcNow.AddDays(random.Next(7, 31)) : (DateTime?)null;
            var isSeasonal = random.Next(0, 2) == 0;

            var quiz = new Quiz(
                title: title,
                description: description,
                xpReward: xpReward,
                bonusXp: bonusXp,
                tier: tier,
                priority: priority,
                deadline: deadline,
                isSeasonal: isSeasonal,
                relatedFeature: feature
            );

            // Add actions based on feature
            AddActionsToQuest(quiz, feature, random);
            quizzes.Add(quiz);
        }

        return quizzes;
    }

    private static string GenerateDescription(string feature, string location, Random random)
    {
        return feature switch
        {
            "QRHunts" => $"اسکن {random.Next(3, 6)} QR از اشیاء تاریخی در {location} برای کسب XP!",
            "LastQuiz" => $"کوئیز نهایی در {location} را تکمیل کنید تا به سطح طلایی برسید!",
            "Strike" => $"برای {random.Next(5, 8)} روز متوالی در {location} فعالیت کنید!",
            "TreasureHunt" => $"گنج مخفی در {location} را پیدا کنید و پاداش بگیرید!",
            "DailyChallenge" => $"چالش روزانه در {location} را تکمیل کنید!",
            _ => $"ماموریت ویژه در {location} برای کسب امتیاز اضافی!"
        };
    }

    private static void AddActionsToQuest(Quiz quiz, string feature, Random random)
    {
        switch (feature)
        {
        case "QRHunts":
            quiz.AddAction("ScanQR", $"اسکن QR اشیاء در {quiz.Title}", random.Next(3, 6));
            break;
        case "LastQuiz":
            quiz.AddAction("CompleteQuiz", $"پاسخ به کوئیز در {quiz.Title}", 1);
            break;
        case "Strike":
            quiz.AddAction("MaintainStreak", $"فعالیت روزانه در {quiz.Title}", random.Next(5, 8));
            break;
        case "TreasureHunt":
            quiz.AddAction("FindTreasure", $"گنج یابی در {quiz.Title}", random.Next(1, 3));
            break;
        case "DailyChallenge":
            quiz.AddAction("CompleteChallenge", $"چالش روزانه در {quiz.Title}", 1);
            break;
        default:
            quiz.AddAction("GeneralAction", $"اقدام عمومی در {quiz.Title}", 1);
            break;
        }
    }
}
