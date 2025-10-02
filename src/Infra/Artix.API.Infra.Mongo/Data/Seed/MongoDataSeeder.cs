namespace Artix.API.Infra.Mongo.Data.Seed;

using Core.Domain.Entities.Quest;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

 

public static class MongoDataSeeder
{
    public static async Task EnsureMongoMigrationAsync(IMongoDatabase database)
    {

        // چک کردن وجود مجموعه quest
        var collectionNames = await (await database.ListCollectionNamesAsync()).ToListAsync();
        if (!collectionNames.Contains("quest"))
        {
            await database.CreateCollectionAsync("quest");
        }

        // ایجاد ایندکس‌ها
        var collection = database.GetCollection<Quest>("quest");
        var indexKeys = Builders<Quest>.IndexKeys
            .Ascending("IsDeleted")
            .Ascending("RelatedFeature")
            .Ascending("Priority");

        var indexModel = new CreateIndexModel<Quest>(indexKeys, new CreateIndexOptions
        {
            Name = "Quest_IsDeleted_RelatedFeature_Priority",
            Background = true
        });

        var existingIndexes = await (await collection.Indexes.ListAsync()).ToListAsync();
        if (!existingIndexes.Any(i => i["name"].AsString == "Quest_IsDeleted_RelatedFeature_Priority"))
        {
            await collection.Indexes.CreateOneAsync(indexModel);
        }
        else
        {
            throw new Exception("Index 'Quest_IsDeleted_RelatedFeature_Priority' already exists.");
        }
    }

    public static async Task SeedQuestsAsync(IMongoDatabase database)
    {

        var collection = database.GetCollection<Quest>("quest");

        // پاک کردن مجموعه برای جلوگیری از خطای DuplicateKey
        await collection.DeleteManyAsync(Builders<Quest>.Filter.Empty);

        // چک کردن اینکه آیا collection خالیه (برای اطمینان)
        var count = await collection.CountDocumentsAsync(Builders<Quest>.Filter.Empty);
        if (count > 0)
        {
            return;
        }

        // ایجاد Questهای نمونه
        var quests = new List<Quest>
        {
            // Quest 1: QR Hunt (اسکن 5 QR در موزه)
            new Quest(
                title: "اسکن 5 QR در موزه تهران",
                description: "5 شیء تاریخی را در موزه تهران اسکن کنید تا XP بگیرید!",
                xpReward: 200,
                bonusXp: 50,
                tier: 1,
                priority: 10,
                deadline: DateTime.UtcNow.AddDays(30),
                isSeasonal: true,
                relatedFeature: "QRHunts"
            ),

            // Quest 2: LastQuiz (کوئیز نهایی برای golden level)
            new Quest(
                title: "تکمیل کوئیز نهایی موزه",
                description: "با اسکن مجدد اشیاء و پیدا کردن شیء مخفی، به golden level برسید!",
                xpReward: 300,
                bonusXp: 100,
                tier: 2,
                priority: 8,
                deadline: null,
                isSeasonal: false,
                relatedFeature: "LastQuiz"
            ),

            // Quest 3: Strike (حفظ توالی فعالیت)
            new Quest(
                title: "حفظ شعله کمپ",
                description: "برای 7 روز متوالی فعالیت کنید تا Strike خود را حفظ کنید!",
                xpReward: 150,
                bonusXp: 25,
                tier: 1,
                priority: 9,
                deadline: DateTime.UtcNow.AddDays(7),
                isSeasonal: true,
                relatedFeature: "Strike"
            )
        };

        // اضافه کردن اقدامات (actions) به Questها
        quests[0].AddAction("ScanQR", "اسکن QR اشیاء موزه تهران", 5);
        quests[1].AddAction("CompleteQuiz", "پاسخ به کوئیز شیء مخفی", 1);
        quests[2].AddAction("MaintainStreak", "فعالیت روزانه برای حفظ Strike", 7);

        // درج مستقیم Questها در MongoDB
        await collection.InsertManyAsync(quests);
    }
}
