namespace Artix.API.Infra.Sql.Data;

using Microsoft.EntityFrameworkCore;

public sealed class ArtixQueryDbContext : DbContext
{
    public ArtixQueryDbContext(DbContextOptions<ArtixQueryDbContext> options)
        : base(options)
    {
    }

    
    #region DbSets

    #endregion


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ArtixQueryDbContext).Assembly);
    }
}
