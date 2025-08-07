namespace Artix.API.Infra.File;

using Core.Contract.Configs.FileStorage;
using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependencyInjection
{
    public static void AddFileService(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind FileStorageOptions
        services.Configure<FileStorageOptions>(configuration.GetSection(FileStorageOptions.SectionName));

        // Validate configuration
        var options = configuration.GetSection(FileStorageOptions.SectionName).Get<FileStorageOptions>();
        if (options == null || string.IsNullOrWhiteSpace(options.Path))
        {
            throw new ArgumentException("FileStorage:Path configuration is missing or empty.", nameof(configuration));
        }

        // Register IFileService with Scoped lifetime
        services.AddScoped<IFileService, FileService>();
    }
}
