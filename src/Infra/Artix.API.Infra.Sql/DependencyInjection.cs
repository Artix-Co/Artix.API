namespace Artix.API.Infra.Sql;

using Core.Contract.Primitives.Repositories;
using Data;
using Data.DbContexts;
using Data.Interceptors;
using Data.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Primitives;

public static class DependencyInjection
{
    public static void AddSqlServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the query-side DbContext (for read operations) without migrations
        services.AddDbContext<ArtixQueryDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("QueryConnectionString"))
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(
                            CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))

            // .EnableSensitiveDataLogging()
            // .EnableDetailedErrors()
        );

        // Register the command-side DbContext (for write operations)
        services.AddDbContext<ArtixCommandDbContext>(options =>
                options
                    .UseSqlServer(configuration.GetConnectionString("CommandConnectionString"))
                    .ConfigureWarnings(warnings =>
                        warnings.Ignore(
                            CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning))
                    .UseLazyLoadingProxies()
            // .EnableSensitiveDataLogging()
            // .EnableDetailedErrors()
        );


        services.AddSingleton<IChangeInterceptor, TimestampInterceptor>();
        services.AddSingleton<IChangeInterceptor, DomainEventInterceptor>();
        services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
        services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddTransient<SqlDataRemover>();
        services.AddTransient<SqlDataSeeder>();
    }
}
