using System.IO.Compression;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Endpoints;
using Artix.API.Infra.Mongo.Data.Seed;
using Artix.API.Infra.RabbitMQ.Services.Notification;
using Artix.API.Infra.Sql.Data.Seed;
using Artix.API.Infra.Sql.Exceptions;
using Artix.API.Orchestration.ServiceDefaults;
using Artix.API.WebService;
using Artix.API.WebService.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Serilog;
using Serilog.Sinks.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);


// ------------------------------------
// Serilog Configuration
// ------------------------------------
builder.Host.UseSerilog((context, services, config) =>
{
    config.ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Artix.API")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);

    var elasticStatus = services.GetService<ElasticsearchStatus>();
    if (elasticStatus?.IsValid == true)
    {
        config.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticStatus.Uri))
        {
            AutoRegisterTemplate = true,
            IndexFormat = elasticStatus.Index,
            ModifyConnectionSettings = c => c
                .BasicAuthentication(elasticStatus.Settings.Username, elasticStatus.Settings.Password)
                .RequestTimeout(TimeSpan.FromMinutes(elasticStatus.Settings.RequestTimeoutInMinutes))
        });
    }
});

builder.AddServiceDefaults();
builder.Services.AddSignalR();

var environment = builder.Environment;
bool isDevelopmentEnv = environment.IsDevelopment();

var keyStorePathKeys = isDevelopmentEnv
    ? "/Users/mohammadnazari/.aspnet/DataProtection-Keys"
    : "/app/dataprotection-keys";

builder.Services.AddDataProtection()
    .SetApplicationName("Artix")
    .PersistKeysToFileSystem(new DirectoryInfo(keyStorePathKeys));


builder.Services.AddArtixServices(builder.Configuration);

// ------------------------------------
// Kestrel
// ------------------------------------
// builder.WebHost.UseKestrel(options =>
// {
//     options.AddServerHeader = false;
//
//     options.Limits.MaxRequestBodySize = 8L * 1024 * 1024 * 1024;
//     options.Limits.MaxConcurrentConnections = 500;
//     options.Limits.MaxConcurrentUpgradedConnections = 50;
//     options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(20);
//     options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
//     options.Limits.MinRequestBodyDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
//     options.Limits.MinResponseDataRate = new MinDataRate(100, TimeSpan.FromSeconds(10));
//     options.Limits.MaxRequestBufferSize = 1024 * 1024;
//     options.Limits.MaxResponseBufferSize = 1024 * 1024;
//     options.AllowSynchronousIO = false;
//
//
//     options.ListenAnyIP(80, listen =>
//     {
//         listen.Protocols = HttpProtocols.Http1AndHttp2;
//         Log.Information("✔ Kestrel listening on port {Port}", 80);
//     });
//
//     if (isDevelopmentEnv)
//     {
//         options.ListenLocalhost(8080, listen => Log.Information("✔ Kestrel listening on port {Port}", 8080));
//         options.ListenLocalhost(7013, listen =>
//         {
//             listen.UseHttps();
//             Log.Information("✔ Kestrel listening on HTTPS port {Port}", 7013);
//         });
//     }
// });

var app = builder.Build();

// --------------------------------------------------
// FINAL — NO ENV — STORAGE PATH NORMALIZATION LOGIC
// --------------------------------------------------
string rawStoragePath = builder.Configuration["FileSettings:StoragePath"] ?? "uploads/files";

static string NormalizePath(string path, string contentRoot)
{
    if (string.IsNullOrWhiteSpace(path))
        return Path.Combine(contentRoot, "uploads", "files");

    path = path.Trim();

    // Expand "~"
    if (path.StartsWith("~"))
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var relative = path.Substring(1).TrimStart('/', '\\');
        return Path.GetFullPath(Path.Combine(home, relative));
    }

    // If relative → make absolute
    if (!Path.IsPathRooted(path))
    {
        return Path.GetFullPath(Path.Combine(contentRoot, path));
    }

    // Already absolute
    return Path.GetFullPath(path);
}

string filesPath = NormalizePath(rawStoragePath, builder.Environment.ContentRootPath);

// Ensure folder exists & writable
try
{
    if (!Directory.Exists(filesPath))
    {
        Log.Warning("Files directory missing: {Path}. Creating...", filesPath);
        Directory.CreateDirectory(filesPath);
    }

    var test = Path.Combine(filesPath, ".write_test");
    File.WriteAllText(test, "ok");
    File.Delete(test);
}
catch (Exception ex)
{
    Log.Fatal(ex, "Cannot access storage folder: {Path}", filesPath);
    throw;
}

Log.Information("✔ FILE STORAGE RESOLVED → {Path}", filesPath);

// --------------------------------------------------
// Add Accept-Ranges for files
// --------------------------------------------------
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/files"))
        ctx.Response.Headers.Append("Accept-Ranges", "bytes");

    await next();
});

// --------------------------------------------------
// Strip query parameters from static file requests
// --------------------------------------------------
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/files"))
    {
        var path = ctx.Request.Path.ToString();
        if (path.Contains('&') && !path.Contains('?'))
        {
            // Find the first & and treat everything after as query string
            var ampIndex = path.IndexOf('&');
            if (ampIndex > 0)
            {
                var newPath = path.Substring(0, ampIndex);
                var queryString = "?" + path.Substring(ampIndex + 1);
                
                ctx.Request.Path = newPath;
                ctx.Request.QueryString = new QueryString(queryString);
                
                Log.Debug("Fixed malformed URL. Path: {NewPath}, Query: {Query}", newPath, queryString);
            }
        }
    }
    
    await next();
});

// --------------------------------------------------
// DB Migrations & Seeding
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var sqlDataRemover = services.GetRequiredService<SqlDataRemover>();
    var sqlDataSeeder = services.GetRequiredService<SqlDataSeeder>();
    var sqlMigration = services.GetRequiredService<SqlMigration>();
    var mongoSeeder = services.GetRequiredService<MongoDataSeeder>();


    await sqlMigration.MigrateAsync();
    // await sqlDataRemover.Remove();
    // await sqlDataSeeder.SeedAsync();


    await mongoSeeder.EnsureMongoMigrationAsync();
    // await mongoSeeder.SeedQuizzesAsync();
}


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        var status = ex switch
        {
            InfrastructureNotFoundException => 404,
            ApplicationServiceNotFoundException => 404,
            _ => 500
        };

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        Log.Error(ex,
            "Unhandled error. Path: {Path}, Status: {StatusCode}",
            feature?.Path, status);

        var baseResponse = new ErrorResponse
        {
            Error = ex?.Message, Exception = ex?.GetType().Name, Status = status, Path = feature?.Path
        };

        // Only include stack trace in Development
        if (isDevelopmentEnv)
        {
            var detailed = new
            {
                baseResponse.Error,
                baseResponse.Exception,
                baseResponse.Status,
                baseResponse.Path,
                Stack = ex?.ToString()
            };

            await context.Response.WriteAsJsonAsync(detailed);
            return;
        }

        // Production: no stack, clean output
        await context.Response.WriteAsJsonAsync(baseResponse);
    });
});

app.UseResponseCompression();

// --------------------------------------------------
// GZip fallback for .gz (ignores query parameters)
// --------------------------------------------------
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/files", out var remaining))
    {
        // Get just the path without query string for file lookup
        var cleanPath = remaining.Value.Split('?')[0].TrimStart('/');
        
        if (cleanPath.Contains(".."))
        {
            ctx.Response.StatusCode = 400;
            return;
        }

        var full = Path.Combine(filesPath, cleanPath);
        var gz = full + ".gz";
        var ext = Path.GetExtension(full).ToLowerInvariant();

        if (!File.Exists(full) && File.Exists(gz))
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = ext switch
            {
                ".glb" => "model/gltf-binary",
                ".gltf" => "model/gltf+json",
                ".json" => "application/json",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            ctx.Response.Headers["Cache-Control"] = "public,max-age=31536000";
            ctx.Response.Headers["Accept-Ranges"] = "bytes";

            await using var fs = File.OpenRead(gz);
            await using var gzip = new GZipStream(fs, CompressionMode.Decompress);
            await gzip.CopyToAsync(ctx.Response.Body);
            return;
        }
    }

    await next();
});

// --------------------------------------------------
// Static Files → /files
// --------------------------------------------------
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

app.UseCustomMiddlewares(app.Environment);

if (true)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();
app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<NotificationHub>("/notificationHub")
    .RequireAuthorization();

app.Run();
