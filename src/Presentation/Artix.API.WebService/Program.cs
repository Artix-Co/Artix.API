using System.Text.Json;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Contract.Primitives.Models;
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
using Elastic.Transport;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using Nest;
using Serilog;
using Serilog.Sinks.Elasticsearch;

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

builder.Services.AddSingleton<IElasticClient>(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var elasticUri = configuration["Elasticsearch:Uri"];
    var username = configuration["Elasticsearch:Username"];
    var password = configuration["Elasticsearch:Password"];
    var indexFormat = configuration["Elasticsearch:IndexFormat"];
    var requestTimeout = configuration["Elasticsearch:RequestTimeoutInMinutes"];

    if (int.TryParse(requestTimeout, out int requestInMinutes))
    {
        var settings = new ConnectionSettings(new Uri(elasticUri))
            .DefaultIndex(indexFormat)
            .RequestTimeout(TimeSpan.FromMinutes(requestInMinutes))
            .BasicAuthentication(username, password);

        return new ElasticClient(settings);
    }

    throw new InvalidOperationException("Invalid Elasticsearch configuration.");
});

var elasticUri = builder.Configuration["Elasticsearch:Uri"];
var username = builder.Configuration["Elasticsearch:Username"];
var password = builder.Configuration["Elasticsearch:Password"];
var indexFormat = builder.Configuration["Elasticsearch:IndexFormat"];
var requestTimeout = builder.Configuration["Elasticsearch:RequestTimeoutInMinutes"];

if (int.TryParse(requestTimeout, out int requestInMinutes))
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticUri))
        {
            AutoRegisterTemplate = true,
            IndexFormat = indexFormat,
            ModifyConnectionSettings = c => c.BasicAuthentication(username, password)
                .RequestTimeout(TimeSpan.FromMinutes(requestInMinutes)),
        })
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
}
else
{
    throw new InvalidOperationException("Invalid Elasticsearch request timeout configuration.");
}

builder.Host.UseSerilog();

builder.AddServiceDefaults();


builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(80, listen => { listen.Protocols = HttpProtocols.Http1AndHttp2; });

    options.AddServerHeader = false;

    // ---- Limits ----
    options.Limits.MaxRequestBodySize = 8L * 1024 * 1024 * 1024; // 8GB
    options.Limits.MaxConcurrentConnections = 500;
    options.Limits.MaxConcurrentUpgradedConnections = 50;

    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(20);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);

    options.Limits.MinRequestBodyDataRate =
        new MinDataRate(100, TimeSpan.FromSeconds(10));

    options.Limits.MinResponseDataRate =
        new MinDataRate(100, TimeSpan.FromSeconds(10));

    options.Limits.MaxRequestBufferSize = 1024 * 1024;
    options.Limits.MaxResponseBufferSize = 1024 * 1024;

    options.AllowSynchronousIO = false;

    if (builder.Environment.IsDevelopment())
    {
        options.ListenLocalhost(5274);
        options.ListenLocalhost(7013, x => x.UseHttps());
    }
});


var app = builder.Build();

Log.Logger.Information("Application built!");

var storagePathConfig = builder.Configuration["FileSettings:StoragePath"] ?? "uploads/files";

string filesPath;

if (Path.IsPathRooted(storagePathConfig))
{
    filesPath = storagePathConfig;
}
else
{
    filesPath = Path.Combine(builder.Environment.ContentRootPath, storagePathConfig);
}

if (!Directory.Exists(filesPath))
{
    Log.Logger.Warning("Files directory does not exist: {Path}. Creating...", filesPath);
    Directory.CreateDirectory(filesPath);
}

Log.Logger.Information("Serving static files from: {FilesPath}", filesPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(filesPath),
    RequestPath = "/files",
    ServeUnknownFileTypes = true,
    DefaultContentType = "application/octet-stream",
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public, max-age=31536000");
        ctx.Context.Response.Headers.Append("Accept-Ranges", "bytes");
    }
});


app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/files"))
    {
        context.Response.Headers.Append("Accept-Ranges", "bytes");
    }

    await next();
});

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

    Log.Information("Truncating and resetting all SQL tables (with FK support)...");

// این روش کاملاً امن و بدون ارور FK هست
    await sqlCommandDbContext.Database.ExecuteSqlRawAsync(@"
    SET NOCOUNT ON;

    -- مرحله ۱: غیرفعال کردن همه Foreign Keyها در کل دیتابیس
    EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL';

    -- مرحله ۲: DELETE به جای TRUNCATE (چون TRUNCATE با FK مشکل داره حتی با NOCHECK در بعضی موارد)
    -- ولی ما DELETE + TRUNCATE ترکیب می‌کنیم برای جداول بدون FK مستقیم + Identity reset
    DELETE FROM dbo.AppVersions; DBCC CHECKIDENT ('dbo.AppVersions', RESEED, 0);
    DELETE FROM dbo.AspNetRoleClaims;
    DELETE FROM dbo.AspNetRoles; DBCC CHECKIDENT ('dbo.AspNetRoles', RESEED, 0);
    DELETE FROM dbo.AspNetUserClaims;
    DELETE FROM dbo.AspNetUserLogins;
    DELETE FROM dbo.AspNetUserRoles;
    DELETE FROM dbo.AspNetUsers; DBCC CHECKIDENT ('dbo.AspNetUsers', RESEED, 0);
    DELETE FROM dbo.AspNetUserTokens;
    DELETE FROM dbo.CollectionItems;
    DELETE FROM dbo.Collections; DBCC CHECKIDENT ('dbo.Collections', RESEED, 0);
    DELETE FROM dbo.Files;
    DELETE FROM dbo.Friendships;
    DELETE FROM dbo.HistoricalPeriods; DBCC CHECKIDENT ('dbo.HistoricalPeriods', RESEED, 0);
    DELETE FROM dbo.JournalEntries;
    DELETE FROM dbo.MarketplaceItems;
    DELETE FROM dbo.MuseumImages;
    DELETE FROM dbo.MuseumObjects;
    DELETE FROM dbo.Museums; DBCC CHECKIDENT ('dbo.Museums', RESEED, 0);
    DELETE FROM dbo.Notifications;
    DELETE FROM dbo.ObjectHistoricalPeriods;
    DELETE FROM dbo.ObjectImages;
    DELETE FROM dbo.ObjectModels;
    DELETE FROM dbo.Objects; DBCC CHECKIDENT ('dbo.Objects', RESEED, 0);
    DELETE FROM dbo.ObjectTypes;
    DELETE FROM dbo.OTPs;
    DELETE FROM dbo.OutboxMessages;
    DELETE FROM dbo.Seasons;
    DELETE FROM dbo.SeasonTasks;
    DELETE FROM dbo.TierConfigs; DBCC CHECKIDENT ('dbo.TierConfigs', RESEED, 0);
    DELETE FROM dbo.Types; DBCC CHECKIDENT ('dbo.Types', RESEED, 0);
    DELETE FROM dbo.UserImages;
    DELETE FROM dbo.UserJournalEntries;
    DELETE FROM dbo.UserLoginHistories;
    DELETE FROM dbo.UserMuseumKeys;
    DELETE FROM dbo.UserNotification;
    DELETE FROM dbo.UserScans;
    DELETE FROM dbo.UserSeasonProgresses;
    DELETE FROM dbo.UserStrikes;
    DELETE FROM dbo.UserXps;
    DELETE FROM dbo.VoiceTrackFiles;
    DELETE FROM dbo.VoiceTracks;

    -- مرحله ۳: دوباره فعال کردن همه FKها
    EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL';
");

    Log.Information("All SQL tables cleaned and identity columns reset successfully!");

    // Seeding داده‌های SQL
    // await SqlDataSeeder.SeedAsync(sqlCommandDbContext, userManager, roleManager);

    // Seeding داده‌های MongoDB
    await MongoDataSeeder.SeedQuizzesAsync(mongoCommandContext);
}

// app.UseExceptionHandler(config =>
// {
//     config.Run(async context =>
//     {
//         var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
//
//         context.Response.ContentType = "application/json";
//
//         context.Response.StatusCode = exception switch
//         {
//             InfrastructureNotFoundException => StatusCodes.Status404NotFound,
//             ApplicationServiceNotFoundException => StatusCodes.Status404NotFound,
//             _ => StatusCodes.Status500InternalServerError
//         };
//
//         var result = JsonSerializer.Serialize(new { error = exception?.Message });
//
//         await context.Response.WriteAsync(result);
//     });
// });

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain; charset=utf-8";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = exceptionHandlerPathFeature?.Error;

        var errorDetails = $"Exception: {ex?.GetType().Name}\n" +
                           $"Message: {ex?.Message}\n" +
                           $"StackTrace:\n{ex?.StackTrace}\n" +
                           $"Path: {exceptionHandlerPathFeature?.Path}";

        await context.Response.WriteAsync(errorDetails);
    });
});
app.UseResponseCompression();
app.UseCustomMiddlewares(app.Environment);

var elasticClient = new ElasticClient(new ConnectionSettings(new Uri(elasticUri))
    .BasicAuthentication(username, password)
    .ServerCertificateValidationCallback(CertificateValidations.AllowAll)
    .RequestTimeout(TimeSpan.FromMinutes(requestInMinutes)));

var pingResponse = await elasticClient.PingAsync();

if (pingResponse.IsValid)
{
    Log.Information("Connected to Elasticsearch");
}
else
{
    Log.Error("Failed to connect to Elasticsearch: {Reason}", pingResponse.OriginalException?.Message);
}

app.MapGet("/elastic-health", () =>
{
    BaseApiResponse<string> result = new BaseApiResponse<string>();
    if (pingResponse.IsValid)
    {
        result.Data = "connected to elasticsearch";
        return result;
    }

    result.Data = "not connected to elasticsearch";
    return result;
});

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
