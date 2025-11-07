namespace Artix.API.Infra.Sql.Data.Interceptors;

using Core.Domain.Entities.Common;
using DbContexts;
using Microsoft.EntityFrameworkCore;

internal sealed class TimestampInterceptor : IChangeInterceptor
{
    public void BeforeSaveChanges(ArtixCommandDbContext context)
    {
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.Entity is BaseEntity &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            switch (entityEntry.State)
            {
            case EntityState.Added:
                entityEntry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = DateTime.UtcNow;
                break;
            case EntityState.Modified:
                entityEntry.Property(nameof(BaseEntity.ModifiedAt)).CurrentValue = DateTime.UtcNow;
                entityEntry.Property(nameof(BaseEntity.CreatedAt)).IsModified = false;
                break;
            }
        }
    }

    public async Task BeforeSaveChangesAsync(ArtixCommandDbContext context, CancellationToken cancellationToken)
    {
        BeforeSaveChanges(context);
        await Task.CompletedTask;
    }
}
