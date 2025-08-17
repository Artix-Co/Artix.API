namespace Artix.ServiceDefaults;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddServiceDefaults(this IHostApplicationBuilder builder)
    {
        // اضافه کردن HealthChecks برای اپلیکیشن‌ها
        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "app" });

        // اضافه کردن HealthCheck برای Redis
        builder.Services.AddHealthChecks()
            .AddRedis(
                redisConnectionString: "redis:6379,password=Heli@ghar771379",
                name: "redis",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "redis" });

        // اضافه کردن HealthCheck برای RabbitMQ
        builder.Services.AddHealthChecks()
            .AddRabbitMQ(
                sp =>
                {
                    var factory = new ConnectionFactory { Uri = new Uri("amqp://admin:admin@rabbitmq:5672/") };
                    return factory.CreateConnectionAsync();
                },
                name: "rabbitmq",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "rabbitmq" });


        // اضافه کردن HealthCheck برای SQL Server
        builder.Services.AddHealthChecks()
            .AddSqlServer(
                connectionString:
                "Server=sqlserver,1433;Database=master;User Id=sa;Password=Hello&Run1234;TrustServerCertificate=True",
                name: "sqlserver",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "sqlserver" });


        // اضافه کردن Service Discovery (اختیاری)
        builder.Services.AddServiceDiscovery();

        return builder;
    }
}
