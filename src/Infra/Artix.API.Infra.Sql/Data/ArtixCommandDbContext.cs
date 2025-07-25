namespace Artix.API.Infra.Sql.Data;

using System.Reflection;
using Core.Domain.Entities._primitives;
using Core.Domain.Entities.Collection;
using Core.Domain.Entities.JournalEntry;
using Core.Domain.Entities.MarketPlace;
using Core.Domain.Entities.Museum;
using Core.Domain.Entities.MusicTrack;
using Core.Domain.Entities.Season;
using Core.Domain.Entities.User;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public sealed class ArtixCommandDbContext : IdentityDbContext<AppUser, AppRole, long>
{
    public ArtixCommandDbContext(DbContextOptions<ArtixCommandDbContext> options)
        : base(options)
    {
    }

    #region DbSets
    public DbSet<AppRole> AppRoles { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    
    public DbSet<Collection> Collections { get; set; }
    public DbSet<CollectionItem> CollectionItems { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<MarketplaceItem> MarketplaceItems { get; set; }
  
    public DbSet<MusicTrack> MusicTracks { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SeasonTask> SeasonTasks { get; set; }
 
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
    public DbSet<Category> Categories { get; set; } 
    public DbSet<MuseumObjectCategory> MuseumObjectCategories { get; set; } 
    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Name.EndsWith("WriteConfiguration"));
    }

    #region SaveChanges

    public override int SaveChanges()
    {
        this.UpdateTimestamps();


        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        this.UpdateTimestamps();


        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = this.ChangeTracker.Entries()
            .Where(e => e is { Entity: BaseEntity, State: EntityState.Added or EntityState.Modified });

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;
            if (entityEntry.State != EntityState.Modified) continue;
            entity.ModifiedAt = DateTime.UtcNow;
            entityEntry.Property(nameof(entity.CreatedAt)).IsModified = false;
        }
    }

    #endregion
}
