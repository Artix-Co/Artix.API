namespace Artix.API.Infra.Sql.Data;

using System.Reflection;
using Core.Domain.Entities.Collection;
using Core.Domain.Entities.JournalEntry;
using Core.Domain.Entities.MarketPlace;
using Core.Domain.Entities.Museum;
using Core.Domain.Entities.MusicTrack;
using Core.Domain.Entities.Season;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class ArtixQueryDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public ArtixQueryDbContext(DbContextOptions<ArtixQueryDbContext> options)
        : base(options)
    {
    }
    
    #region DbSets
    
    public DbSet<Collection> Collections { get; set; }
    public DbSet<CollectionItem> CollectionItems { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
    public DbSet<Museum> Museums { get; set; }
    public DbSet<MuseumObject> MuseumObjects { get; set; }
    public DbSet<MusicTrack> MusicTracks { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SeasonTask> SeasonTasks { get; set; }
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<UserJournalEntry> UserJournalEntries { get; set; }
    public DbSet<UserMuseumKey> UserMuseumKeys { get; set; }
    public DbSet<UserObject> UserObjects { get; set; }
    public DbSet<UserSeasonProgress> UserSeasonProgresses { get; set; }
    public DbSet<UserStrike> UserStrikes { get; set; }
    public DbSet<UserTrack> UserTracks { get; set; }
    public DbSet<UserXp> UserXps { get; set; }
    
    public DbSet<Category> Categories { get; set; } 
    public DbSet<MuseumObjectCategory> MuseumObjectCategories { get; set; } 
    
    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Name.EndsWith("ReadConfiguration"));
    }
}
