namespace Artix.API.Infra.Sql.Data.Seed;

using Core.Domain.Entities.Museum;
using Core.Domain.Entities.User;
using DbContexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        const int MUEUM_SEED_COUNT = 7;
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
                                               string.Join(", ",
                                                   roleCreateResult.Errors.Select(e => e.Description)));
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

        #endregion


        #region Seed Categories (assuming categories are needed for MuseumObjectCategory)

        var categories = new List<Type>();

        for (int i = 0; i < CATEGORY_SEED_COUNT; i++)
        {
            var category = Type.Create($"Fake category {i}", $"Fake description category {i}");
            categories.Add(category);
        }


        context.Categories.AddRange(categories);

        #endregion


        #region Seed Museums

        var museums = new List<Museum>();
        for (int i = 0; i < MUEUM_SEED_COUNT; i++)
        {
            var museum = Museum.Create($"Fake museum {i}", $"A collection of fine arts, fake data {i}",
                isActive: true);
            museums.Add(museum);
        }


        context.Museums.AddRange(museums);

        #endregion


        #region Seed MuseumObjects (managed through Museum aggregate root)

        foreach (var museum in museums)
        {
            var museumObjects = new List<MuseumObject>
            {
                MuseumObject.Create(
                    name: "Ancient Vase",
                    qrCode: "QR_VASE_001",
                    museum: museum,
                    isSpecial: true,
                    isHidden: false
                ),
                MuseumObject.Create(
                    name: "Mona Lisa",
                    qrCode: "QR_MONA_001",
                    museum: museum,
                    isSpecial: true,
                    isHidden: false
                ),
                MuseumObject.Create(
                    name: "Bronze Statue",
                    qrCode: "QR_STATUE_001",
                    museum: museum,
                    isSpecial: false,
                    isHidden: true
                )
            };

            museumObjects.AddRange(museumObjects);

            foreach (var museumObject in museumObjects)
                museumObject.Museum.AddObject(museumObject);


            #region Seed MuseumObjectCategories

            var museumObjectCategories = new List<ObjectType>
            {
                ObjectType.Create(museumObjects[0], categories[0]),
                ObjectType.Create(museumObjects[1], categories[1]),
                ObjectType.Create(museumObjects[2], categories[2])
            };
            context.MuseumObjectCategories.AddRange(museumObjectCategories);

            #endregion
        }

        #endregion


        await context.SaveChangesAsync();
    }

    private static async Task<bool> IsDatabaseSeededAsync(ArtixCommandDbContext context)
    {
        return await context.Users.AnyAsync();
    }
}
