using System.IO.Compression;
using System.Text.Json;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Core.Domain.Entities.User;
using Artix.API.Endpoints;
using Artix.API.Infra.Sql.Data.DbContexts;
using Artix.API.Infra.Sql.Data.Seed;
using Artix.API.Infra.Sql.Exceptions;
using Artix.API.WebService;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;




if (environment.IsProduction())
{
    // Register Redis distributed lock factory (RedLock)
    builder.Services.AddSingleton<IDistributedLockFactory>(_ =>
    {
        var redisEndpoints = new[]
        {
            new RedLockEndPoint { EndPoint = new DnsEndPoint("redis", 6379) } // نام سرویس Redis در داکر-کامپوز
        };
        return RedLockFactory.Create(redisEndpoints);
    });

    // Register a singleton state to track leadership
    builder.Services.AddSingleton<LeaderState>();

    
    builder.Services.AddDataProtection()
        .SetApplicationName("Artix")
        .PersistKeysToFileSystem(new DirectoryInfo("/app/dataprotection-keys"));
}

builder.Services.AddHealthChecks();

// فعال‌سازی فشرده‌سازی پاسخ‌ها (Gzip)
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // فعال‌سازی برای HTTPS
    options.Providers.Add<GzipCompressionProvider>(); // استفاده از Gzip
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat([
        "application/json"
    ]);
});


// تنظیم سطح فشرده‌سازی Gzip
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal; // تعادل بین سرعت و میزان فشرده‌سازی
});

builder.Services.AddArtixServices(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

Log.Logger.Information("Application built!");

if (environment.IsProduction())
{
    var leaderState = app.Services.GetRequiredService<LeaderState>();
    var lockFactory = app.Services.GetRequiredService<IDistributedLockFactory>();

// Background task to try acquiring leadership lock periodically
    _ = Task.Run(async () =>
    {
        while (true)
        {
            using var lockHandle = await lockFactory.CreateLockAsync("leader-lock", TimeSpan.FromSeconds(10));
            if (lockHandle.IsAcquired)
            {
                leaderState.IsLeader = true;
                Console.WriteLine("🔵 I am the leader!");
                await Task.Delay(5000);
            }
            else
            {
                leaderState.IsLeader = false;
                Console.WriteLine("🟡 Standby...");
                await Task.Delay(3000);
            }
        }
    });

// ساده‌ترین endpoint برای تست وضعیت لاک
    app.MapGet("/lock",
        (LeaderState leader) =>
        {
            return leader.IsLeader ? Results.Ok("I'm the leader and I serve this.") : Results.StatusCode(503);
        });

// Endpoint اصلی که فقط نود Leader اجازه داره جواب بده
    app.MapGet("/process", (LeaderState leader) =>
    {
        if (!leader.IsLeader)
            return Results.StatusCode(503);

        var id = Guid.NewGuid();
        Console.WriteLine($"✔️ Leader handled the request: {id}");
        return Results.Ok($"Handled by leader: {id}");
    });
}



using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();


    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context, userManager, roleManager);
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


if (environment.IsDevelopment())  
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/elastic-health", async (IElasticClient elasticClient) =>
{
    var response = await elasticClient.PingAsync();

    BaseApiResponse<string> result = new BaseApiResponse<string>();


    if (response.IsValid)
    {
        result.IsSuccess = true;
        result.Data = "connected to elasticsearch";
        return result;
    }

    result.IsSuccess = false;
    result.Data = "not connected to elasticsearch";
    return result;
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(new { status = report.Status.ToString(), details = report.Entries });
        await context.Response.WriteAsync(json);
    }
});


app.UseResponseCaching();
app.Use(async (context, next) =>
{
    if (context.Request.Method == "GET")
    {
        context.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
        {
            Public = true, MaxAge = TimeSpan.FromSeconds(60) // کش 60 ثانیه‌ای برای پاسخ‌ها
        };
    }

    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();


record LeaderState
{
    public volatile bool IsLeader;
}
