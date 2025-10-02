namespace Artix.API.Infra.Mongo.Data.Seed;

using Core.Domain.Entities.Quest;
 
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MongoDataSeeder
{
    public static async Task SeedQuestsAsync(IMongoDatabase database)
    {

        var collection = database.GetCollection<Quest>(typeof(Quest).Name.ToLowerInvariant());

        // چک کردن اینکه آیا collection خالیه (برای جلوگیری از seed تکراری)
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
                priority: 10, // اولویت بالا برای نمایش
                deadline: DateTime.UtcNow.AddDays(30), // مهلت 30 روزه
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
                deadline: null, // بدون مهلت
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

        // ایجاد Indexها
        var indexKeys = Builders<Quest>.IndexKeys
            .Ascending("IsDeleted")
            .Ascending("RelatedFeature")
            .Ascending("Priority");

        var indexModel = new CreateIndexModel<Quest>(indexKeys, new CreateIndexOptions
        {
            Name = "Quest_IsDeleted_RelatedFeature_Priority",
            Background = true // برای performance بهتر در production
        });

        await collection.Indexes.CreateOneAsync(indexModel);
    }
}
