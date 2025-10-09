using Microsoft.EntityFrameworkCore;
using Newspoint.Application.Services;
using Newspoint.Infrastructure.Database;
using Newspoint.Infrastructure.Database.Seeders;

namespace Newspoint.Server.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["Database:ConnectionStrings:MySql"];
        var serverVersion = ServerVersion.AutoDetect(connectionString);

        services.AddDbContext<DbContext, DataDbContext>(options =>
            options.UseMySql(
                connectionString,
                serverVersion,
                o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

        services.AddScoped<DatabaseSeeder>();
        
        return services;
    }

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
}