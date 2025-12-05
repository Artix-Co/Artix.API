namespace Artix.API.Infra.Sql.Data.Seed;

using Core.Domain.Entities.Museum;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.Domain.Entities.Object;
using Core.Domain.Entities.Object.Enums;
using Core.Domain.Entities.Object.ValueObjects;
using Core.Domain.Entities.TierConfig;
using Core.Domain.Entities.User.Enums;
using Core.Domain.Entities.Version;
using DbContexts;
using Microsoft.Extensions.Logging;
using Object = Core.Domain.Entities.Object.Object;

public class SqlDataSeeder
{
    private const int USER_SEED_COUNT = 7;
    private const int MUSEUM_SEED_COUNT = 7;
    private const int CATEGORY_SEED_COUNT = 7;
    private const int OBJECT_SEED_COUNT = 7;
    private const int HISTORICAL_PERIOD_SEED_COUNT = 7;
    private const int APP_VERSION_SEED_COUNT = 4;

    private readonly ArtixCommandDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly ILogger<SqlDataSeeder> _logger;

    public SqlDataSeeder(
        ArtixCommandDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        ILogger<SqlDataSeeder> logger)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        using var activity = new Activity("SqlDataSeeder.SeedAll").Start();
        _logger.LogInformation("SqlDataSeeder | Starting SQL data seeding process");

        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("SqlDataSeeder | SQL migrations applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlDataSeeder | Failed to apply SQL migrations");
            throw;
        }

        if (await _context.Users.AnyAsync())
        {
            _logger.LogWarning("SqlDataSeeder | Database already contains users → Skipping entire seeding process");
            return;
        }

        _logger.LogInformation("SqlDataSeeder | Database is empty → Starting full seeding operation");

        try
        {
            await SeedRolesAsync();
            await SeedUsersAndFriendshipsAsync();
            await SeedCategoriesAsync();
            await SeedHistoricalPeriodsAsync();
            await SeedMuseumsAsync();
            await SeedObjectsAsync();
            await SeedObjectTypesAsync();
            await SeedObjectHistoricalPeriodsAsync();
            await SeedMuseumObjectsAsync();
            await SeedAppVersionsAsync();
            await SeedTierConfigsAsync();

            await _context.SaveChangesAsync();
            _logger.LogInformation("SqlDataSeeder | All entities seeded and changes saved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlDataSeeder | Error during seeding process");
            throw;
        }

        _logger.LogWarning("SqlDataSeeder | SQL data seeding completed successfully");
    }

    private async Task SeedRolesAsync()
    {
        var roles = Enum.GetNames(typeof(Role)).ToList();
        _logger.LogDebug("SqlDataSeeder | Starting role seeding | Total roles to check: {RoleCount}", roles.Count);

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                _logger.LogInformation("SqlDataSeeder | Role '{Role}' does not exist → Creating new role", role);
                var roleResult = await _roleManager.CreateAsync(new AppRole(role));
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("SqlDataSeeder | Failed to create role '{Role}': {Errors}", role,
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    throw new ApplicationException($"Failed to create role {role}");
                }

                _logger.LogInformation("SqlDataSeeder | Role '{Role}' created successfully", role);
            }
            else
            {
                _logger.LogDebug("SqlDataSeeder | Role '{Role}' already exists", role);
            }
        }
    }

    private async Task SeedUsersAndFriendshipsAsync()
    {
        var clientTypes = Enum.GetNames(typeof(ClientType)).ToList();
        var users = new List<AppUser>();
        var friendships = new List<Friendship>();

        _logger.LogDebug("SqlDataSeeder | Starting user seeding | Target count: {UserCount}", USER_SEED_COUNT);

        for (int i = 0; i < USER_SEED_COUNT; i++)
        {
            var user = new AppUser
            {
                UserName = $"username{i}",
                Email = $"username{i}@gmail.com",
                PhoneNumber = "0987654321",
                DisplayName = $"Fake User {i}"
            };

            _logger.LogInformation("SqlDataSeeder | Creating user '{Username}'", user.UserName);
            var createResult = await _userManager.CreateAsync(user, "Heli@ghar771379");
            if (!createResult.Succeeded)
            {
                _logger.LogError("SqlDataSeeder | User creation failed for '{Username}': {Errors}", user.UserName,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                throw new ApplicationException($"User creation failed for {user.UserName}");
            }

            var role = i == 0 ? "Admin" : "Client";
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("SqlDataSeeder | Role assignment failed for '{Username}': {Errors}", user.UserName,
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                throw new ApplicationException($"Role assignment failed for {user.UserName}");
            }

            if (i != 0)
            {
                var clientType = clientTypes[i % clientTypes.Count];
                var claimResult =
                    await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("ClientType", clientType));
                if (!claimResult.Succeeded)
                {
                    _logger.LogError("SqlDataSeeder | Claim assignment failed for '{Username}': {Errors}",
                        user.UserName, string.Join(", ", claimResult.Errors.Select(e => e.Description)));
                    throw new ApplicationException($"Claim assignment failed for {user.UserName}");
                }
            }

            users.Add(user);
            _logger.LogInformation("SqlDataSeeder | User '{Username}' created with role '{Role}'", user.UserName, role);
        }

        _logger.LogDebug("SqlDataSeeder | Starting friendship seeding");
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

        _context.Friendships.AddRange(friendships);
        _logger.LogInformation("SqlDataSeeder | Seeded {UserCount} users and {FriendshipCount} friendships",
            users.Count, friendships.Count);
    }

    private async Task SeedCategoriesAsync()
    {
        var categories = new List<Category>();
        _logger.LogDebug("SqlDataSeeder | Starting category seeding | Target count: {CategoryCount}",
            CATEGORY_SEED_COUNT);

        for (int i = 0; i < CATEGORY_SEED_COUNT; i++)
        {
            var category = Category.Create($"Fake category {i}", $"Fake description category {i}");
            categories.Add(category);
        }

        _context.Types.AddRange(categories);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} categories", categories.Count);
    }

    private async Task SeedHistoricalPeriodsAsync()
    {
        var historicalPeriods = new List<HistoricalPeriod>();
        _logger.LogDebug("SqlDataSeeder | Starting historical periods seeding");

        historicalPeriods.AddRange(new[]
        {
            HistoricalPeriod.Create("Roman Era", "Artifacts from the Roman Empire (100–400 AD)",
                new HistoricalDate(100, 1, 1), new HistoricalDate(400, 12, 31)),
            HistoricalPeriod.Create("Renaissance", "Art from the Renaissance period (1300–1600 AD)",
                new HistoricalDate(1300, 1, 1), new HistoricalDate(1600, 12, 31)),
            HistoricalPeriod.Create("Greek Period", "Artifacts from ancient Greece (800–100 BC)",
                new HistoricalDate(-800, 1, 1), new HistoricalDate(-100, 1, 1))
        });

        _context.HistoricalPeriods.AddRange(historicalPeriods.Take(HISTORICAL_PERIOD_SEED_COUNT));
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} historical periods", historicalPeriods.Count);
    }

    private async Task SeedMuseumsAsync()
    {
        var museums = new List<Museum>();
        _logger.LogDebug("SqlDataSeeder | Starting museum seeding | Target count: {MuseumCount}", MUSEUM_SEED_COUNT);

        for (int i = 0; i < MUSEUM_SEED_COUNT; i++)
        {
            var museum = Museum.Create($"Fake museum {i}", $"A collection of fine arts, fake data {i}", isActive: true);
            museums.Add(museum);
        }

        _context.Museums.AddRange(museums);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} museums", museums.Count);
    }

    private async Task SeedObjectsAsync()
    {
        var objects = new List<Object>();
        _logger.LogDebug("SqlDataSeeder | Starting object seeding");

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
        _context.Objects.AddRange(objects);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} objects", objects.Count);
    }

    private async Task SeedObjectTypesAsync()
    {
        var objectTypes = new List<ObjectType>();
        var categories = await _context.Types.ToListAsync();
        var objects = await _context.Objects.ToListAsync();

        _logger.LogDebug("SqlDataSeeder | Starting object types seeding");

        for (int i = 0; i < objects.Count; i++)
        {
            var objectType = ObjectType.Create(objects[i], categories[i % categories.Count]);
            objectTypes.Add(objectType);
        }

        _context.ObjectTypes.AddRange(objectTypes);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} object types", objectTypes.Count);
    }

    private async Task SeedObjectHistoricalPeriodsAsync()
    {
        var objectHistoricalPeriods = new List<ObjectHistoricalPeriod>();
        var historicalPeriods = await _context.HistoricalPeriods.ToListAsync();
        var objects = await _context.Objects.ToListAsync();

        _logger.LogDebug("SqlDataSeeder | Starting object historical periods seeding");

        for (int i = 0; i < objects.Count; i++)
        {
            var objectHistoricalPeriod =
                ObjectHistoricalPeriod.Create(objects[i], historicalPeriods[i % historicalPeriods.Count]);
            objectHistoricalPeriods.Add(objectHistoricalPeriod);
        }

        _context.ObjectHistoricalPeriods.AddRange(objectHistoricalPeriods);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} object historical periods",
            objectHistoricalPeriods.Count);
    }

    private async Task SeedMuseumObjectsAsync()
    {
        var museums = await _context.Museums.ToListAsync();
        var objects = await _context.Objects.ToListAsync();

        _logger.LogDebug("SqlDataSeeder | Starting museum objects assignment");

        await _context.SaveChangesAsync();

        for (int i = 0; i < objects.Count; i++)
        {
            var museum = museums[i % museums.Count];
            objects[i].AssignMuseum(museum.Id);
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("SqlDataSeeder | Assigned objects to museums successfully");
    }

    private async Task SeedAppVersionsAsync()
    {
        var appVersions = new List<AppVersion>();
        _logger.LogDebug("SqlDataSeeder | Starting app versions seeding");

        appVersions.AddRange(new[]
        {
            AppVersion.Create(1, 0, 0, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 1, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 2, true, false, "First Version On Development Environment"),
            AppVersion.Create(1, 0, 3, true, true, "First Version On Development Environment")
        });

        _context.AppVersions.AddRange(appVersions.Take(APP_VERSION_SEED_COUNT));
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} app versions", appVersions.Count);
    }

    private async Task SeedTierConfigsAsync()
    {
        var tierConfigs = new List<TierConfig>();
        _logger.LogDebug("SqlDataSeeder | Starting tier configs seeding");

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

        _context.TierConfigs.AddRange(tierConfigs);
        _logger.LogInformation("SqlDataSeeder | Seeded {Count} tier configs", tierConfigs.Count);
    }
}
