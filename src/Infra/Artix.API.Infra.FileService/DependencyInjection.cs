namespace Artix.API.Infra.FileService;

using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependencyInjection
{
    public static void AddFileService(this IServiceCollection services)
    {
        services.AddSingleton<IUploadRepository, InMemoryUploadRepository>();
        services.AddSingleton<IFileStorage, FileSystemStorage>();
        services.AddScoped<IUploadService, UploadService>();
        // services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));
    }
}
