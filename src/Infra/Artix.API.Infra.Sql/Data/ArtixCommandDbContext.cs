namespace Artix.API.Infra.Sql.Data;

using Core.Domain.Entities._primitives;
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

    #endregion


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtixCommandDbContext).Assembly);
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
