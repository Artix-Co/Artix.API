using System.Text.Json;
using Artix.API.Core.ApplicationService.Exceptions;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Endpoints;
using Artix.API.Infra.Sql.Data;
using Artix.API.Infra.Sql.Data.DbContexts;
using Artix.API.Infra.Sql.Data.Seed;
using Artix.API.Infra.Sql.Exceptions;
using Artix.API.WebService;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Nest;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddArtixServices(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

Log.Logger.Information("Application built!");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ArtixCommandDbContext>();
    await context.Database.MigrateAsync();
    // await DataSeeder.SeedAsync(context);
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

        var result = JsonSerializer.Serialize(new
        {
            error = exception?.Message
        });

        await context.Response.WriteAsync(result);
    });
});

app.UseCustomMiddlewares(app.Environment);

Log.Logger.Information("Application started!");
if (app.Environment.IsDevelopment())
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


app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
