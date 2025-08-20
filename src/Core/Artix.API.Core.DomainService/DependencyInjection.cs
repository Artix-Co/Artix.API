namespace Artix.API.Core.DomainService;

using Interfaces.Notification;
using Microsoft.Extensions.DependencyInjection;
using Services.Notification;

public static class DependencyInjection
{
    public static void AddDomainServiceServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationServiceProvider, NotificationServiceProvider>();
    }
}
