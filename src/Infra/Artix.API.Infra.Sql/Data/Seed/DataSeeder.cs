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
        const int USER_SEED_COUNT = 100;
        const int MENU_SEED_COUNT = 100;
        try
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            #region user seeder

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
            for (int i = 0; i < 100; i++)
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

            // Seed Friendships (connect each user to every other user)
            var friendships = new List<Friendship>();
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = i + 1; j < users.Count; j++)
                {
                    friendships.Add(Friendship.Create(users[i], users[j]));
                    friendships.Add(Friendship.Create(users[j], users[i]));
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

            var categories = new List<Category>
            {
                Category.Create("Historical"), Category.Create("Art"), Category.Create("Archaeological")
            };
            context.Categories.AddRange(categories);

            #endregion


            #region Seed Museums

            var museums = new List<Museum>
            {
                Museum.Create("National History Museum", "A museum of historical artifacts", isActive: true),
                Museum.Create("Art Gallery", "A collection of fine arts", isActive: true)
            };
            context.Museums.AddRange(museums);

            #endregion


            #region Seed MuseumObjects (managed through Museum aggregate root)

            var museumObjects = new List<MuseumObject>
            {
                MuseumObject.Create(
                    name: "Ancient Vase",
                    qrCode: "QR_VASE_001",
                    museum: museums[0],
                    isSpecial: true,
                    isHidden: false
                ),
                MuseumObject.Create(
                    name: "Mona Lisa",
                    qrCode: "QR_MONA_001",
                    museum: museums[0],
                    isSpecial: true,
                    isHidden: false
                ),
                MuseumObject.Create(
                    name: "Bronze Statue",
                    qrCode: "QR_STATUE_001",
                    museum: museums[0],
                    isSpecial: false,
                    isHidden: true
                )
            };


            foreach (var museumObject in museumObjects)
                museumObject.Museum.AddObject(museumObject);

            #endregion


            #region Seed MuseumObjectCategories

            var museumObjectCategories = new List<MuseumObjectCategory>
            {
                MuseumObjectCategory.Create(museumObjects[0], categories[0]), // Ancient Vase -> Historical
                MuseumObjectCategory.Create(museumObjects[1], categories[1]), // Mona Lisa -> Art
                MuseumObjectCategory.Create(museumObjects[2], categories[2]) // Bronze Statue -> Archaeological
            };
            context.MuseumObjectCategories.AddRange(museumObjectCategories);

            #endregion 
          
            
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}
