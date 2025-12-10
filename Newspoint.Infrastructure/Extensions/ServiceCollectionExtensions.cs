using Microsoft.Extensions.DependencyInjection;
using Newspoint.Domain.Interfaces;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositoriesFromAssembly(this IServiceCollection services)
    {
        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(IRepository), typeof(DataDbContext))
                .AddClasses(c => c.AssignableTo(typeof(IRepository)))
                .AsImplementedInterfaces()
                .WithScopedLifetime();
        });
        return services;
    }
}
