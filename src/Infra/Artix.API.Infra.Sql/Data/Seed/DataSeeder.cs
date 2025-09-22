namespace Artix.API.Infra.Sql.Data.Seed;

using Type = Core.Domain.Entities.Object.Type;
using Object = Core.Domain.Entities.Object.Object;
using Core.Domain.Entities.Museum;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.Object;
using Core.Domain.Entities.Object.Enums;
using Core.Domain.Entities.Tier;
using Core.Domain.Entities.User.Enums;
using Core.Domain.Entities.Version;
using Core.Domain.ValueObjects;
using DbContexts;

public static class DataSeeder
{
    private const int USER_SEED_COUNT = 7;
    private const int MUSEUM_SEED_COUNT = 7;
    private const int CATEGORY_SEED_COUNT = 7;
    private const int OBJECT_SEED_COUNT = 3;
    private const int HISTORICAL_PERIOD_SEED_COUNT = 3;
    private const int APP_VERSION_SEED_COUNT = 4;

    public static async Task SeedAsync(ArtixCommandDbContext context, UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        // Check if database is already seeded
        if (await context.Users.AnyAsync())
        {
            return; // Skip seeding if any users exist
        }

        var roles = Enum.GetNames(typeof(Role)).ToList();
        var clientTypes = Enum.GetNames(typeof(ClientType)).ToList();
        var users = new List<AppUser>();
        var friendships = new List<Friendship>();
        var categories = new List<Type>();
        var historicalPeriods = new List<HistoricalPeriod>();
        var museums = new List<Museum>();
        var objects = new List<Object>();
        var objectTypes = new List<ObjectType>();
        var objectHistoricalPeriods = new List<ObjectHistoricalPeriod>();
        var appVersions = new List<AppVersion>();
        var tierConfigs = new List<TierConfig>();

        #region Seed Roles

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var roleResult = await roleManager.CreateAsync(new AppRole(role));
                if (!roleResult.Succeeded)
                    throw new ApplicationException(
                        $"Failed to create role {role}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
            }
        }

        #endregion

        #region Seed Users and Friendships

        for (int i = 0; i < USER_SEED_COUNT; i++)
        {
            var user = new AppUser
            {
                UserName = $"username{i}",
                Email = $"username{i}@gmail.com",
                PhoneNumber = "0987654321",
                DisplayName = $"Fake User {i}"
            };

            var createResult = await userManager.CreateAsync(user, "Heli@ghar771379");
            if (!createResult.Succeeded)
                throw new ApplicationException(
                    $"User creation failed for {user.UserName}: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");

            var role = i == 0 ? "Admin" : "Client";
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new ApplicationException(
                    $"Role assignment failed for {user.UserName}: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

            if (i != 0)
            {
                var clientType = clientTypes[i % clientTypes.Count];
                var claimResult =
                    await userManager.AddClaimAsync(user, new System.Security.Claims.Claim("ClientType", clientType));
                if (!claimResult.Succeeded)
                    throw new ApplicationException(
                        $"Claim assignment failed for {user.UserName}: {string.Join(", ", claimResult.Errors.Select(e => e.Description))}");
            }

            users.Add(user);
        }

        // Create friendships efficiently
        for (int i = 0; i < users.Count; i += 2)
        {
            if (i + 1 < users.Count)
            {
                var user1 = users[i];
                var user2 = users[i + 1];
                friendships.Add(Friendship.Create(user1, user2));
                friendships.Add(Friendship.Create(user2, user1));
            }
        }

        context.Friendships.AddRange(friendships);

        #endregion

        #region Seed Categories

        for (int i = 0; i < CATEGORY_SEED_COUNT; i++)
        {
            categories.Add(Type.Create($"Fake category {i}", $"Fake description category {i}"));
        }

        context.Types.AddRange(categories);

        #endregion

        #region Seed Historical Periods

        historicalPeriods.AddRange(new[]
        {
            HistoricalPeriod.Create("Roman Era", "Artifacts from the Roman Empire (100–400 AD)",
                new HistoricalDate(100, 1, 1), new HistoricalDate(400, 12, 31)),
            HistoricalPeriod.Create("Renaissance", "Art from the Renaissance period (1300–1600 AD)",
                new HistoricalDate(1300, 1, 1), new HistoricalDate(1600, 12, 31)),
            HistoricalPeriod.Create("Greek Period", "Artifacts from ancient Greece (800–100 BC)",
                new HistoricalDate(-800, 1, 1), new HistoricalDate(-100, 1, 1))
        });
        context.HistoricalPeriods.AddRange(historicalPeriods.Take(HISTORICAL_PERIOD_SEED_COUNT));

        #endregion

        #region Seed Museums

        for (int i = 0; i < MUSEUM_SEED_COUNT; i++)
        {
            museums.Add(Museum.Create($"Fake museum {i}", $"A collection of fine arts, fake data {i}", isActive: true));
        }

        context.Museums.AddRange(museums);

        #endregion

        #region Seed Objects

        objects.AddRange(new[]
        {
            Object.Create("Ancient Vase", "QR_VASE_001", "A vase from the Roman era",
                "Made of clay with intricate designs", 1, 2, true, false, ObjectSaleType.Free),
            Object.Create("Mona Lisa", "QR_MONA_001", "Famous painting by Leonardo da Vinci",
                "Iconic portrait with a mysterious smile", 1, 3, true, false, ObjectSaleType.Tokenized),
            Object.Create("Bronze Statue", "QR_STATUE_001", "A statue from the Greek period",
                "Depicts a warrior in battle pose", 1, 1, false, true, ObjectSaleType.MemberShip)
        });
        objects = objects.Take(OBJECT_SEED_COUNT).ToList();
        context.Objects.AddRange(objects);

        #endregion

        #region Seed ObjectTypes

        for (int i = 0; i < objects.Count; i++)
        {
            objectTypes.Add(ObjectType.Create(objects[i], categories[i % categories.Count]));
        }

        context.ObjectTypes.AddRange(objectTypes);

        #endregion

        #region Seed ObjectHistoricalPeriods

        for (int i = 0; i < objects.Count; i++)
        {
            objectHistoricalPeriods.Add(ObjectHistoricalPeriod.Create(objects[i],
                historicalPeriods[i % historicalPeriods.Count]));
        }

        context.ObjectHistoricalPeriods.AddRange(objectHistoricalPeriods);

        #endregion

        #region Seed MuseumObjects

        // برای جلوگیری از خطا، ابتدا موزه‌ها و آبجکت‌ها را ذخیره می‌کنیم تا Idهای واقعی تولید شوند
        await context.SaveChangesAsync();

        for (int i = 0; i < objects.Count; i++)
        {
            var museum = museums[i % museums.Count];
            objects[i].AssignMuseum(museum.Id);
        }

        // ذخیره تغییرات نهایی (شامل روابط)
        await context.SaveChangesAsync();

        #endregion

        #region Seed AppVersions

        appVersions.AddRange(new[]
        {
            AppVersion.Create(1, 0, 0, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 1, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 2, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 3, true, true, "First Version On Development Environment")
        });
        context.AppVersions.AddRange(appVersions.Take(APP_VERSION_SEED_COUNT));

        #endregion


        #region Seed TierConfigs
        tierConfigs.AddRange(new[]
        {
            TierConfig.Create(
                minScanCount: 1,
                requiredUpgraded: false,
                requiredInCollection: false,
                minDaysSinceAcquired: 0,
                requiredSpecial: false,
                requiredSaleType: null,
                requiredMembershipType: "First Version On Development Environment"
            ),
            TierConfig.Create(
                minScanCount: 1,
                requiredUpgraded: false,
                requiredInCollection: true,
                minDaysSinceAcquired: 0,
                requiredSpecial: false,
                requiredSaleType: null,
                requiredMembershipType: "First Version On Development Environment"
            ),
            TierConfig.Create(
                minScanCount: 1,
                requiredUpgraded: false,
                requiredInCollection: false,
                minDaysSinceAcquired: 2,
                requiredSpecial: false,
                requiredSaleType: null,
                requiredMembershipType: "First Version On Development Environment"
            ),
            TierConfig.Create(
                minScanCount: 1,
                requiredUpgraded: false,
                requiredInCollection: false,
                minDaysSinceAcquired: 3,
                requiredSpecial: true,
                requiredSaleType: null,
                requiredMembershipType: "First Version On Development Environment"
            )
        });

        context.TierConfigs.AddRange(tierConfigs);
        #endregion


        #region Final Save for remaining entities

        await context.SaveChangesAsync();

        #endregion
    }
}
