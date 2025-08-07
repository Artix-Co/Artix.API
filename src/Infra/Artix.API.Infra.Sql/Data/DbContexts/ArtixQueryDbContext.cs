namespace Artix.API.Infra.Sql.Data.DbContexts;

using System.Reflection;
using Artix.API.Core.Domain.Entities.Collection;
using Artix.API.Core.Domain.Entities.JournalEntry;
using Artix.API.Core.Domain.Entities.Museum;
using Artix.API.Core.Domain.Entities.Season;
using Artix.API.Core.Domain.Entities.User;
using Core.Domain.Entities.File;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class ArtixQueryDbContext : IdentityDbContext<AppUser, AppRole, long,
    IdentityUserClaim<long>,
    IdentityUserRole<long>,
    IdentityUserLogin<long>,
    IdentityRoleClaim<long>,
    IdentityUserToken<long>>
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

    public DbSet<Museum> Museums { get; set; }
    public DbSet<MuseumObject> MuseumObjects { get; set; }
    public DbSet<Type> Types { get; set; }
    
    public DbSet<Object> Objects { get; set; }
    public DbSet<ObjectType> ObjectTypes { get; set; }
    public DbSet<HistoricalPeriod> HistoricalPeriods { get; set; }
    public DbSet<ObjectHistoricalPeriod> ObjectHistoricalPeriods { get; set; }
    public DbSet<FileEntity> Files { get; set; }

    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Name.EndsWith("ReadConfiguration"));
    }
}
