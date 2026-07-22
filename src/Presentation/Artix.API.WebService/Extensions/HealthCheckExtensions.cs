namespace Artix.API.WebService.Extensions;

using System.Text.Json;
using Infra.Sql.Data.DbContexts;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using StackExchange.Redis;

public static class HealthCheckExtensions
{
    private static readonly string[] LiveTags = ["live"];
    private static readonly string[] ReadyTags = ["ready"];
    private static readonly TimeSpan CheckTimeout = TimeSpan.FromSeconds(5);

    public static IServiceCollection AddArtixHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: LiveTags)
            .AddDbContextCheck<ArtixQueryDbContext>(
                name: "sqlserver",
                tags: ReadyTags)
            .Add(new HealthCheckRegistration(
                "mongodb",
                sp => new MongoDbHealthCheck(sp.GetRequiredService<IMongoClient>()),
                failureStatus: HealthStatus.Unhealthy,
                tags: ReadyTags,
                timeout: CheckTimeout))
            .Add(new HealthCheckRegistration(
                "redis",
                sp => new RedisHealthCheck(sp.GetRequiredService<IConnectionMultiplexer>()),
                failureStatus: HealthStatus.Unhealthy,
                tags: ReadyTags,
                timeout: CheckTimeout))
            .Add(new HealthCheckRegistration(
                "rabbitmq",
                sp => new RabbitMqHealthCheck(sp.GetRequiredService<IConnection>()),
                failureStatus: HealthStatus.Unhealthy,
                tags: ReadyTags,
                timeout: CheckTimeout));

        return services;
    }

    public static Task WriteHealthResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var payload = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                durationMs = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            }));
    }
}

file sealed class MongoDbHealthCheck(IMongoClient client) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await client
                .GetDatabase("admin")
                .RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("MongoDB unreachable.", ex);
        }
    }
}

file sealed class RedisHealthCheck(IConnectionMultiplexer multiplexer) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!multiplexer.IsConnected)
                return HealthCheckResult.Unhealthy("Redis multiplexer is disconnected.");

            var latency = await multiplexer.GetDatabase().PingAsync();
            return HealthCheckResult.Healthy($"ping {latency.TotalMilliseconds:F0}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis unreachable.", ex);
        }
    }
}

file sealed class RabbitMqHealthCheck(IConnection connection) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!connection.IsOpen)
                return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ connection is closed."));

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy("RabbitMQ unreachable.", ex));
        }
    }
}
