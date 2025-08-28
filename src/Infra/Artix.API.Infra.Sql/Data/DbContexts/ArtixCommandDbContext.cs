namespace Artix.API.Infra.Sql.Data.DbContexts;

using System.Reflection;
using System.Text.Json;
using Artix.API.Core.Domain.Entities.Collection;
using Artix.API.Core.Domain.Entities.Common;
using Artix.API.Core.Domain.Entities.JournalEntry;
using Artix.API.Core.Domain.Entities.Museum;
using Artix.API.Core.Domain.Entities.Season;
using Artix.API.Core.Domain.Entities.User;
using Core.Domain.Entities.File;
using Core.Domain.Entities.Notification;
using Core.Domain.Entities.Object;
using Core.Domain.Entities.OTP;
using Core.Domain.Entities.Version;
using Core.Domain.Entities.Voice;
using Core.Domain.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

public sealed class ArtixCommandDbContext : IdentityDbContext<AppUser, AppRole, long,
    IdentityUserClaim<long>,
    IdentityUserRole<long>,
    IdentityUserLogin<long>,
    IdentityRoleClaim<long>,
    AppUserToken>

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

    public DbSet<VoiceTrack> MusicTracks { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<SeasonTask> SeasonTasks { get; set; }

    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<UserJournalEntry> UserJournalEntries { get; set; }
    public DbSet<UserMuseumKey> UserMuseumKeys { get; set; }
    public DbSet<UserObject> UserObjects { get; set; }
    public DbSet<UserSeasonProgress> UserSeasonProgresses { get; set; }
    public DbSet<UserStrike> UserStrikes { get; set; }

    public DbSet<UserXp> UserXps { get; set; }
    public DbSet<OTP> OTPs { get; set; }
    public DbSet<UserLoginHistory> UserLoginHistories { get; set; }


    public DbSet<Museum> Museums { get; set; }
    public DbSet<MuseumObject> MuseumObjects { get; set; }
    public DbSet<MuseumImage> MuseumImages { get; set; }
    public DbSet<Type> Types { get; set; }
    public DbSet<File> Files { get; set; }
    public DbSet<Object> Objects { get; set; }
    public DbSet<Object3DModel> Object3DModels { get; set; }
    public DbSet<ObjectImage> ObjectImages { get; set; }
    public DbSet<ObjectType> ObjectTypes { get; set; }
    public DbSet<HistoricalPeriod> HistoricalPeriods { get; set; }
    public DbSet<ObjectHistoricalPeriod> ObjectHistoricalPeriods { get; set; }
    public DbSet<VoiceTrack> VoiceTracks { get; set; }
    public DbSet<VoiceTrackFile> VoiceTrackFiles { get; set; }
    public DbSet<AppVersion> AppVersions { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            type => type.Name.EndsWith("WriteConfiguration"));
    }


    public override int SaveChanges()
    {
        UpdateTimestamps();
        ProcessDomainEvents();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateTimestamps();
        ProcessDomainEvents();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entityEntry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entityEntry.Property(nameof(BaseEntity.ModifiedAt)).CurrentValue = DateTime.UtcNow;
                entityEntry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
            }
        }
    }

    private void ProcessDomainEvents()
    {
        var aggregates = ChangeTracker.Entries<AggregateRoot>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();


        foreach (var aggregate in aggregates)
        {
            foreach (var @event in aggregate.DomainEvents)
            {
                var outboxMessage = new OutboxMessage
                {
                    Type = @event.GetType().AssemblyQualifiedName!,
                    Data = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        Formatting = Formatting.Indented
                    }),
                    Status = "Pending",
                    CreatedAt = DateTime.UtcNow
                };
                OutboxMessages.Add(outboxMessage);
            }

            aggregate.ClearDomainEvents();
        }
    }
}
