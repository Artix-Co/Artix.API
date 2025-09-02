namespace Artix.API.Core.DomainService;

using Interfaces.FileProcessing;
using Interfaces.Notification;
using Interfaces.XPRules;
using Microsoft.Extensions.DependencyInjection;
using Services.FileProcessing;
using Services.Notification;
using Services.XPRules;

public static class DependencyInjection
{
    public static void AddDomainServiceServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationServiceProvider, NotificationServiceProvider>();
        services.AddScoped<IXpRulesService, XpRulesService>();
        services.AddScoped<IFileProcessingService, FileProcessingService>();
    }
}
