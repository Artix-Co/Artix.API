using System.Text.Json;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Domain.Entities.User;
using Artix.API.Endpoints;
using Artix.API.Infra.RabbitMQ.Services.Notification;
using Artix.API.Infra.Redis.Services.LeaderElection;
using Artix.API.Infra.Sql.Data.DbContexts;
using Artix.API.Infra.Sql.Data.Seed;
using Artix.API.Infra.Sql.Exceptions;
using Artix.API.WebService;
using Artix.ServiceDefaults;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection;
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
var app = builder.Build();

Log.Logger.Information("Application built!");


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
// app.UseCustomMiddlewares(app.Environment);

Log.Logger.Information("Application started!");


if (environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}


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
app.UseWebSockets();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");
app.MapControllers();


app.MapHub<NotificationHub>("/notificationHub");


// Define endpoints
app.MapGet("/lock",
    (LeaderState leader) =>
    {
        return leader.IsLeader ? Results.Ok("I'm the leader and I serve this.") : Results.StatusCode(503);
    });

app.MapGet("/process", (LeaderState leader) =>
{
    if (!leader.IsLeader)
        return Results.StatusCode(503);

    var id = Guid.NewGuid();
    Console.WriteLine($"✔️ Leader handled the request: {id}");
    return Results.Ok($"Handled by leader: {id}");
});

app.Run();
