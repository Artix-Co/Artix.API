namespace Artix.API.Infra.File;

using Core.Contract.Configs.FileSettings;
using Core.Contract.Primitives.Infra.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependencyInjection
{
    public static void AddFileService(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
    }


}
