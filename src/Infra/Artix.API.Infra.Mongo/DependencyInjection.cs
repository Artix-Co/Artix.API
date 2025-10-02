namespace Artix.API.Infra.Mongo;

using Core.Contract.Configs.Mongo;
using Core.Contract.Primitives.Repositories;
using Data.DbContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Primitives;

public static class DependencyInjection
{
    public static void AddMongoServices(this IServiceCollection services, IConfiguration configuration)
    {
   
        services.AddSingleton<IMongoClient>(sp =>
        {
            var mongoSetting = configuration.GetSection("MongoOptions").Get<MongoDbSettings>();
            return new MongoClient(mongoSetting.ConnectionString);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            
            var mongoSetting = configuration.GetSection("MongoOptions").Get<MongoDbSettings>();
         
            
            if (mongoSetting == null || string.IsNullOrEmpty(mongoSetting.DatabaseName))
            {
                Console.WriteLine("MongoOptions.DatabaseName is not configured properly in appsettings.json");
                throw new InvalidOperationException("MongoOptions.DatabaseName is missing in configuration.");
            }

            return client.GetDatabase(mongoSetting.DatabaseName);
        });

        services.AddSingleton<MongoContext>();
        services.AddScoped(typeof(ICommandRepository<>), typeof(MongoCommandRepository<>));
        services.AddScoped(typeof(IQueryRepository<>), typeof(MongoQueryRepository<>));
        services.AddScoped<IUnitOfWork, MongoUnitOfWork>();
    }
}
