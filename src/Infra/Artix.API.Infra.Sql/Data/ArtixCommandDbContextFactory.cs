namespace Artix.API.Infra.Sql.Data;

using DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public sealed class ArtixCommandDbContextFactory : IDesignTimeDbContextFactory<ArtixCommandDbContext>
{
    public ArtixCommandDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ArtixCommandDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1434;Database=ArtixDb;User Id=sa;Password=Hello&Run1234;TrustServerCertificate=True;");

        return new ArtixCommandDbContext(optionsBuilder.Options);
    }
}
