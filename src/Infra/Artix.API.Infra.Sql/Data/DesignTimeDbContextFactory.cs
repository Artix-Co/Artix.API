namespace Artix.API.Infra.Sql.Data;

using DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ArtixCommandDbContext>
{
    public ArtixCommandDbContext CreateDbContext(string[] args)
    {
        var environmentName = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

        var basePath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../Presentation/Artix.API.WebService"));

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CommandConnectionString")
                               ?? throw new InvalidOperationException("CommandConnectionString is not set in appsettings.json.");

        var optionsBuilder = new DbContextOptionsBuilder<ArtixCommandDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new ArtixCommandDbContext(optionsBuilder.Options);
    }
}
