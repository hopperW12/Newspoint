using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Interfaces;
using Newspoint.Infrastructure.Database;
using Newspoint.Infrastructure.Database.Seeders;
using Newspoint.Infrastructure.Repositories;
using Newspoint.Server.Services;

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

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var secret = configuration["Auth:Secret"] ?? "739f17f4b258de7e39184e84bcea413b";
        var key = Encoding.ASCII.GetBytes(secret);
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,     
                    ValidateAudience = false,   
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });
        
        return services;
    }
    
    public static IServiceCollection AddNewspointSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Newspoint API",
                Version = "v1",
                Description = "API dokumentace pro Newspoint Server"
            });
        });

        return services;
    }
    
    public static IServiceCollection AddWebServicesFromAssembly(this IServiceCollection services)
    {
        services.AddScoped<IArticleImageService, ArticleImageService>();
        return services;
    }
}