namespace Artix.API.Infra.Mongo;

using Artix.API.Core.Contract.Configs.Mongo;
using Artix.API.Core.Contract.Primitives.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;
using Data.DbContext;
using Data.Interceptors;
using Data.Seed;
using Primitives;

public static class DependencyInjection
{
    public static void AddMongoServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register MongoClient
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoSetting = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            if (mongoSetting == null || string.IsNullOrEmpty(mongoSetting.ConnectionString) || string.IsNullOrEmpty(mongoSetting.DatabaseName))
            {
                throw new InvalidOperationException("MongoDbSettings is missing or incomplete in configuration.");
            }

            var clientSettings = MongoClientSettings.FromConnectionString(mongoSetting.ConnectionString);
            return new MongoClient(clientSettings);
        });

        // Register MongoDatabase
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var mongoSetting = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            if (mongoSetting == null || string.IsNullOrEmpty(mongoSetting.DatabaseName))
            {
                throw new InvalidOperationException("MongoDbSettings.DatabaseName is missing in configuration.");
            }

            return client.GetDatabase(mongoSetting.DatabaseName);
        });

        // Register Interceptor
        services.AddSingleton<IMongoInterceptor>(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase("YourDatabaseName");
            return new MongoTimestampInterceptor(database);
        });

        // Register Contexts
        services.AddSingleton<MongoQueryContext>();
        services.AddSingleton<MongoCommandContext>();

        // Register Generic Repositories for CQRS
        services.AddScoped(typeof(ICommandRepository<>), typeof(MongoCommandRepository<>));
        services.AddScoped(typeof(IQueryRepository<>), typeof(MongoQueryRepository<>));

        // Register UnitOfWork
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
       services.AddTransient<MongoDataSeeder>();  
 
    }
}

 
