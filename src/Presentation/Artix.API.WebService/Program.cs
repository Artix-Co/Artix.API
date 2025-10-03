using System.Text.Json;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Domain.Entities.User;
using Artix.API.Endpoints;
using Artix.API.Infra.Mongo.Data.DbContext;
using Artix.API.Infra.Mongo.Data.Seed;
using Artix.API.Infra.RabbitMQ.Services.Notification;
using Artix.API.Infra.Sql.Data.DbContexts;
using Artix.API.Infra.Sql.Data.Seed;
using Artix.API.Infra.Sql.Exceptions;
using Artix.API.Orchestration.ServiceDefaults;
using Artix.API.WebService;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();


var environment = builder.Environment;


var keyStorePathKeys = environment.IsDevelopment()
    ? "/Users/mohammadnazari/.aspnet/DataProtection-Keys"
    : "/app/dataprotection-keys";

builder.Services.AddDataProtection()
    .SetApplicationName("Artix")
    .PersistKeysToFileSystem(new DirectoryInfo(keyStorePathKeys));


builder.Services.AddArtixServices(builder.Configuration);

builder.Host.UseSerilog();

builder.AddServiceDefaults();

// Configure Kestrel for high concurrency
builder.WebHost.UseKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = null;
    options.Limits.MaxConcurrentUpgradedConnections = null;
    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(65);
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024; // 10MB
});
var app = builder.Build();

Log.Logger.Information("Application built!");

 

// Perform seeding for MongoDB and SQL
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var mongoDatabase = services.GetRequiredService<IMongoDatabase>();
    var sqlCommandDbContext = services.GetRequiredService<ArtixCommandDbContext>();
    var mongoCommandContext = services.GetRequiredService<MongoCommandContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<AppRole>>();
    

    // اعمال migrationهای SQL
    try
    {
        await sqlCommandDbContext.Database.MigrateAsync();
        Log.Information("SQL migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to apply SQL migrations.");
        throw;
    }

    // چک کردن و اعمال "migration" برای MongoDB
    await MongoDataSeeder.EnsureMongoMigrationAsync(mongoDatabase);

    // Seeding داده‌های SQL
    await SqlDataSeeder.SeedAsync(sqlCommandDbContext, userManager, roleManager);

    // Seeding داده‌های MongoDB
    await MongoDataSeeder.SeedQuestsAsync(mongoCommandContext);
}

app.UseExceptionHandler(config =>
{
    config.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

        context.Response.ContentType = "application/json";

        context.Response.StatusCode = exception switch
        {
            InfrastructureNotFoundException => StatusCodes.Status404NotFound,
            ApplicationServiceNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var result = JsonSerializer.Serialize(new { error = exception?.Message });

        await context.Response.WriteAsync(result);
    });
});
app.UseResponseCompression();
app.UseCustomMiddlewares(app.Environment);

Log.Logger.Information("Application started!");


if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.UseResponseCaching();


app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();


app.MapHub<NotificationHub>("/notificationHub");


app.Run();
