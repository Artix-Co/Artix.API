namespace Artix.API.Infra.Mongo.Data.Seed;

using Core.Domain.Entities.Quest;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DbContext;

public static class MongoDataSeeder
{
    public static async Task EnsureMongoMigrationAsync(IMongoDatabase database)
    {
        // چک کردن وجود مجموعه quest
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!collectionNames.Contains("Quests"))
        {
            await database.CreateCollectionAsync("Quests");
        }

        // ایجاد ایندکس‌ها
        var collection = database.GetCollection<Quest>("Quests");
        var indexKeys = Builders<Quest>.IndexKeys
            .Ascending("IsDeleted")
            .Ascending("RelatedFeature")
            .Ascending("Priority");

        var indexModel = new CreateIndexModel<Quest>(indexKeys,
            new CreateIndexOptions { Name = "Quest_IsDeleted_RelatedFeature_Priority", Background = true });

        var existingIndexes = await (await collection.Indexes.ListAsync()).ToListAsync();
        if (!existingIndexes.Any(i => i["name"].AsString == "Quest_IsDeleted_RelatedFeature_Priority"))
        {
            await collection.Indexes.CreateOneAsync(indexModel);
        }
    }

    public static async Task SeedQuestsAsync(MongoCommandContext commandContext)
    {
        var collection =
            commandContext.GetCollection<Quest>("Quests"); // Note: Changed to plural "quests" to match MongoQueryContext convention

        // Clear collection to avoid duplicate key errors
        await collection.DeleteManyAsync(Builders<Quest>.Filter.Empty);


        // Generate 10 sample quests
        var quests = GenerateSampleQuests(10);

        // Insert quests into MongoDB
        await commandContext.InsertManyAsync(quests);
    }

    private static List<Quest> GenerateSampleQuests(int count)
    {
        var quests = new List<Quest>();
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

            var quest = new Quest(
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
            AddActionsToQuest(quest, feature, random);
            quests.Add(quest);
        }

        return quests;
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

    private static void AddActionsToQuest(Quest quest, string feature, Random random)
    {
        switch (feature)
        {
        case "QRHunts":
            quest.AddAction("ScanQR", $"اسکن QR اشیاء در {quest.Title}", random.Next(3, 6));
            break;
        case "LastQuiz":
            quest.AddAction("CompleteQuiz", $"پاسخ به کوئیز در {quest.Title}", 1);
            break;
        case "Strike":
            quest.AddAction("MaintainStreak", $"فعالیت روزانه در {quest.Title}", random.Next(5, 8));
            break;
        case "TreasureHunt":
            quest.AddAction("FindTreasure", $"گنج یابی در {quest.Title}", random.Next(1, 3));
            break;
        case "DailyChallenge":
            quest.AddAction("CompleteChallenge", $"چالش روزانه در {quest.Title}", 1);
            break;
        default:
            quest.AddAction("GeneralAction", $"اقدام عمومی در {quest.Title}", 1);
            break;
        }
    }
}
