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
using Artix.API.WebService.Extensions;
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


Log.Information("Serilog fully configured with appsettings.json overrides");

builder.AddServiceDefaults();
builder.Services.AddSignalR();

var environment = builder.Environment;
bool isDevelopmentEnv = environment.IsDevelopment();

builder.Services.AddArtixServices(builder.Configuration, isDevelopmentEnv);


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

    if (isDevelopmentEnv)
    {
        options.ListenLocalhost(5274);
        options.ListenLocalhost(7013, x => x.UseHttps());
    }
});


var app = builder.Build();

Log.Information("Application built successfully!");

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

    var sqlDataRemover = services.GetRequiredService<SqlDataRemover>();
    var sqlDataSeeder = services.GetRequiredService<SqlDataSeeder>();
    var mongoSeeder = services.GetRequiredService<MongoDataSeeder>();

    // await sqlDataRemover.Remove();
    await sqlDataSeeder.SeedAsync();

    await mongoSeeder.EnsureMongoMigrationAsync();
    await mongoSeeder.SeedQuizzesAsync();
}


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var ex = feature?.Error;

        var statusCode = ex switch
        {
            InfrastructureNotFoundException => StatusCodes.Status404NotFound,
            ApplicationServiceNotFoundException => StatusCodes.Status404NotFound,

            // اگر Exception لایه اپلیکیشن StatusCode دارد → بخوان
            // برای Result Pattern چون اکثراً exception نمی‌اندازد، فقط روی Exception ها کار می‌کند.

            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        // always log
        Log.Error(ex,
            "Unhandled exception. Path: {Path}, StatusCode: {StatusCode}",
            feature?.Path, statusCode);

        var response = new
        {
            error = ex?.Message,
            exception = ex?.GetType().Name,
            status = statusCode,
            path = feature?.Path,

#if DEBUG
            stackTrace = ex?.StackTrace
#endif
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    });
});

app.UseResponseCompression();
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


Log.Logger.Information("Application started!");


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


app.MapHub<NotificationHub>("/notificationHub");


app.Run();
