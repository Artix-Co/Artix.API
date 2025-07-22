using System.Text.Json;
using Artix.API.Core.Contract.Primitives.Models;
using Artix.API.Endpoints;
using Artix.API.Webservice1;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using Nest;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
builder.Services.AddArtixServices(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();


Log.Logger.Information("Application built!");

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


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast");


app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
