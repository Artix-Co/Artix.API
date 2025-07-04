namespace Artix.API.Core.Contract;

using Primitives.Repositories;
using Primitives.Validations;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddContractServices(this IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // Register MediatR handlers correctly
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assemblies));

        // Register all repositories
        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryRepository<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        // Add MediatR pipeline behavior for FluentValidation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FluentValidationBehavior<,>));
    }
}
