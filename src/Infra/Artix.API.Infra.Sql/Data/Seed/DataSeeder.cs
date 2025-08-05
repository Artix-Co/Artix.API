namespace Artix.API.Infra.Sql.Data.Seed;
using Type = Core.Domain.Entities.Museum.Type;
using Object = Core.Domain.Entities.Museum.Object;

 
 
using Artix.API.Core.Domain.Entities.Museum;
using Artix.API.Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.ValueObjects;
using DbContexts;

public static class DataSeeder
{
    public static async Task SeedAsync(ArtixCommandDbContext context, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        // Check if database is already seeded
        if (await IsDatabaseSeededAsync(context))
        {
            return; // Skip seeding if any table has data
        }

        const int USER_SEED_COUNT = 7;
        const int MUSEUM_SEED_COUNT = 7;
        const int CATEGORY_SEED_COUNT = 7;

        if (context == null) throw new ArgumentNullException(nameof(context));

        #region Seed Users and Friendship

        const string clientRole = "Client";
        var roleExists = await roleManager.RoleExistsAsync(clientRole);
        if (!roleExists)
        {
            var roleCreateResult = await roleManager.CreateAsync(new AppRole(clientRole));
            if (!roleCreateResult.Succeeded)
                throw new ApplicationException("Failed to create Client role: " +
                                               string.Join(", ", roleCreateResult.Errors.Select(e => e.Description)));
        }

        var users = new List<AppUser>();
        for (int i = 0; i < USER_SEED_COUNT; i++)
        {
            var newUser = new AppUser
            {
                UserName = $"username{i}",
                Email = $"username{i}@gmail.com",
                PhoneNumber = "0987654321",
                DisplayName = $"Fake User {i}"
            };

            var createResult = await userManager.CreateAsync(newUser, "Heli@ghar771379");
            var roleResult = await userManager.AddToRoleAsync(newUser, clientRole);

            if (!createResult.Succeeded)
                throw new ApplicationException("User creation failed: " +
                                               string.Join(", ", createResult.Errors.Select(e => e.Description)));

            if (!roleResult.Succeeded)
                throw new ApplicationException("Role assignment failed: " +
                                               string.Join(", ", roleResult.Errors.Select(e => e.Description)));

            users.Add(newUser);
        }

        var friendships = new List<Friendship>();
        foreach (var oddUser in users.Where(u => u.Id % 2 != 0))
        {
            foreach (var evenUser in users.Where(u => u.Id % 2 == 0))
            {
                friendships.Add(Friendship.Create(oddUser, evenUser));
                friendships.Add(Friendship.Create(evenUser, oddUser));
            }
        }

        foreach (var friendship in friendships)
        {
            if (!await context.Friendships.AnyAsync(f =>
                    f.UserId == friendship.UserId && f.FriendId == friendship.FriendId))
            {
                context.Friendships.Add(friendship);
            }
        }

        await context.SaveChangesAsync();

        #endregion

        #region Seed Categories

        var categories = new List<Type>();
        for (int i = 0; i < CATEGORY_SEED_COUNT; i++)
        {
            var category = Type.Create($"Fake category {i}", $"Fake description category {i}");
            categories.Add(category);
        }

        context.Types.AddRange(categories);
        await context.SaveChangesAsync(); // Save to generate Category IDs

        #endregion

        #region Seed HistoricalPeriods

        var historicalPeriods = new List<HistoricalPeriod>
        {
            HistoricalPeriod.Create("Roman Era", "Artifacts from the Roman Empire (100–400 AD)", new HistoricalDate(100, 1, 1), new HistoricalDate(400, 12, 31)),
            HistoricalPeriod.Create("Renaissance", "Art from the Renaissance period (1300–1600 AD)", new HistoricalDate(1300, 1, 1), new HistoricalDate(1600, 12, 31)),
            HistoricalPeriod.Create("Greek Period", "Artifacts from ancient Greece (800–100 BC)", new HistoricalDate(-800, 1, 1), new HistoricalDate(-100, 1, 1))
        };

        context.HistoricalPeriods.AddRange(historicalPeriods);
        await context.SaveChangesAsync(); // Save to generate HistoricalPeriod IDs

        #endregion

        #region Seed Museums

        var museums = new List<Museum>();
        for (int i = 0; i < MUSEUM_SEED_COUNT; i++)
        {
            var museum = Museum.Create($"Fake museum {i}", $"A collection of fine arts, fake data {i}", isActive: true);
            museums.Add(museum);
        }

        context.Museums.AddRange(museums);
        await context.SaveChangesAsync(); // Save to generate Museum IDs

        #endregion

        #region Seed Objects

        var objects = new List<Object>
        {
            Object.Create(
                name: "Ancient Vase",
                qrCode: "QR_VASE_001",
                generalInformation: "A vase from the Roman era",
                specialInformation: "Made of clay with intricate designs",
                version: 1,
                tier: 2,
                isSpecial: true,
                isHidden: false,
                model3DBase64: "base64_vase"
            ),
            Object.Create(
                name: "Mona Lisa",
                qrCode: "QR_MONA_001",
                generalInformation: "Famous painting by Leonardo da Vinci",
                specialInformation: "Iconic portrait with a mysterious smile",
                version: 1,
                tier: 3,
                isSpecial: true,
                isHidden: false,
                model3DBase64: "base64_mona"
            ),
            Object.Create(
                name: "Bronze Statue",
                qrCode: "QR_STATUE_001",
                generalInformation: "A statue from the Greek period",
                specialInformation: "Depicts a warrior in battle pose",
                version: 1,
                tier: 1,
                isSpecial: false,
                isHidden: true,
                model3DBase64: "base64_statue"
            )
        };

        context.Objects.AddRange(objects);
        await context.SaveChangesAsync(); // Save to generate Object IDs

        #endregion

        #region Seed ObjectTypes

        var objectTypes = new List<ObjectType>();
        for (int i = 0; i < objects.Count; i++)
        {
            var objectType = ObjectType.Create(objects[i], categories[i % categories.Count]);
            objectTypes.Add(objectType);
        }

        context.ObjectTypes.AddRange(objectTypes);
        await context.SaveChangesAsync(); // Save to generate ObjectType relationships

        #endregion

        #region Seed ObjectHistoricalPeriods

        var objectHistoricalPeriods = new List<ObjectHistoricalPeriod>();
        for (int i = 0; i < objects.Count; i++)
        {
            var objectHistoricalPeriod = ObjectHistoricalPeriod.Create(objects[i], historicalPeriods[i % historicalPeriods.Count]);
            objectHistoricalPeriods.Add(objectHistoricalPeriod);
        }

        context.ObjectHistoricalPeriods.AddRange(objectHistoricalPeriods);
        await context.SaveChangesAsync(); // Save to generate ObjectHistoricalPeriod relationships

        #endregion

        #region Seed MuseumObjects

        var museumObjects = new List<MuseumObject>();
        for (int i = 0; i < objects.Count; i++)
        {
            var museum = museums[i % museums.Count]; // Assign each object to one museum
            var museumObject = MuseumObject.Create(
                obj: objects[i],
                museum: museum,
                qrCode: objects[i].QrCode,
                isSpecial: objects[i].IsSpecial,
                isHidden: objects[i].IsHidden
            );
            museumObjects.Add(museumObject);
            museum.AddObject(objects[i], museumObject.QRCode, museumObject.IsSpecial, museumObject.IsHidden);
        }

        context.MuseumObjects.AddRange(museumObjects);
        await context.SaveChangesAsync(); // Save MuseumObjects

        #endregion
    }

    private static async Task<bool> IsDatabaseSeededAsync(ArtixCommandDbContext context)
    {
        return await context.Users.AnyAsync() ||
               await context.Museums.AnyAsync() ||
               await context.Objects.AnyAsync();
    }
}
