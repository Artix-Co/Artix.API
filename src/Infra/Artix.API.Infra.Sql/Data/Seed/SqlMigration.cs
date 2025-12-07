namespace Artix.API.Infra.Sql.Data.Seed;

using System.Diagnostics;
using DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class SqlMigration
{
    private readonly ArtixCommandDbContext _context;
    private readonly ILogger<SqlMigration> _logger;

    public SqlMigration(ArtixCommandDbContext context, ILogger<SqlMigration> logger)
    {
        this._context = context;
        this._logger = logger;
    }


    public async Task MigrateAsync()
    {
        using var activity = new Activity("SqlDataSeeder.SeedAll").Start();
        _logger.LogInformation("SqlDataSeeder | Starting SQL data seeding process");


        try
        {
            await _context.Database.MigrateAsync();
            _logger.LogInformation("SqlDataSeeder | SQL migrations applied successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SqlDataSeeder | Failed to apply SQL migrations");
            throw;
        }
    }
}
