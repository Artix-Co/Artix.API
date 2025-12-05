namespace Artix.API.Infra.FileService;

using System.Threading.Channels;
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependencyInjection
{
    public static void AddFileService(this IServiceCollection services)
    {
       
       services.AddHostedService<FileWatcherService>();
       services.AddHostedService<CompressionWorker>();
       services.AddSingleton<IFileCompressor, FileCompressor>();


        services.AddSingleton<IUploadRepository, InMemoryUploadRepository>();
        services.AddSingleton<IFileStorage, FileSystemStorage>();
        services.AddScoped<IUploadService, UploadService>();
        // services.Configure<StorageOptions>(builder.Configuration.GetSection("StorageOptions"));
    }
}
