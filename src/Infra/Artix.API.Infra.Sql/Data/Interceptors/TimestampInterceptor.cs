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

    public async Task BeforeSaveChangesAsync(ArtixCommandDbContext context, CancellationToken cancellationToken)
    {
        BeforeSaveChanges(context); // منطق ناهمزمان اضافی ندارد، فقط از متد همگام استفاده می‌کنیم
        await Task.CompletedTask;
    }
}
