using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Application.Validation;

namespace Newspoint.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServicesFromAssembly(this IServiceCollection services)
    {
        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(IService))
                .AddClasses(c => c.AssignableTo(typeof(IService)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });
        return services;
    }

    public static IServiceCollection AddValidationFromAssembly(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<UserValidator>();
        services.AddFluentValidationAutoValidation();
        return services;
    }
}
