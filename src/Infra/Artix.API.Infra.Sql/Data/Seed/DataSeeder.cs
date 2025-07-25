namespace Artix.API.Infra.Sql.Data.Seed;

using Core.Domain.Entities.Museum;
using Core.Domain.Entities.User;
using DbContexts;
using Microsoft.EntityFrameworkCore;

public static class DataSeeder
{
    public static async Task SeedAsync(ArtixCommandDbContext context)
    {
        try
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            // Seed AppUsers
            var users = new List<AppUser>
            {
                new AppUser
                {
                    UserName = "user1@example.com", Email = "user1@example.com", DisplayName = "User One"
                },
                new AppUser
                {
                    UserName = "user2@example.com", Email = "user2@example.com", DisplayName = "User Two"
                }
            };

            foreach (var user in users)
            {
                if (!await context.AppUsers.AnyAsync(u => u.UserName == user.UserName))
                {
                    context.AppUsers.Add(user);
                }
            }


            // Seed Friendships
            var friendships = new List<Friendship>
            {
                Friendship.Create(users[0], users[1]), Friendship.Create(users[1], users[0])
            };

            foreach (var friendship in friendships)
            {
                if (!await context.Friendships.AnyAsync(f =>
                        f.UserId == friendship.UserId && f.FriendId == friendship.FriendId))
                {
                    context.Friendships.Add(friendship);
                }
            }


            // Seed Categories
            var categories = new List<Category>
            {
                Category.Create("Historical", "Artifacts from historical periods"),
                Category.Create("Art", "Artistic works and paintings"),
                Category.Create("Archaeological", "Items from archaeological digs")
            };

            context.Categories.AddRange(categories);


            // Seed Museums
            var museums = new List<Museum>
            {
                Museum.Create("National History Museum", "A museum of historical artifacts", isActive: true),
                Museum.Create("Art Gallery", "A collection of fine arts", isActive: true)
            };
            context.Museums.AddRange(museums);


            // Seed MuseumObjects
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
                ),
            };

        

   
            context.MuseumObjects.Add(museumObjects[0]);
            

            var museumObjectCategories = new List<MuseumObjectCategory>
            {
                MuseumObjectCategory.Create(museumObjects[0], categories[0]), // Ancient Vase -> Historical
                MuseumObjectCategory.Create(museumObjects[1], categories[1]), // Mona Lisa -> Art
                MuseumObjectCategory.Create(museumObjects[2], categories[2]), // Bronze Statue -> Archaeological
            };

            context.MuseumObjectCategories.Add(museumObjectCategories[0]);
            await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}
