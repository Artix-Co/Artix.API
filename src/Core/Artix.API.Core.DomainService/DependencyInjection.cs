namespace Artix.API.Core.DomainService;

using Contract.Primitives.DomainServices.Notification;
using Contract.Primitives.DomainServices.OTP;
using Contract.Primitives.DomainServices.TierCalculator;
using Contract.Primitives.DomainServices.XPRules;
using Microsoft.Extensions.DependencyInjection;
using Services;

public static class DependencyInjection
{
    public static void AddDomainServiceServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationServiceProvider, NotificationServiceProvider>();
        services.AddScoped<ITierCalculatorService, TierCalculatorService>();
        services.AddScoped<IXpRulesService, XpRulesService>();
        services.AddScoped<IOtpService, OtpService>();
    }
}
