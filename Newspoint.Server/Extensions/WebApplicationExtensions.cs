using Microsoft.EntityFrameworkCore;
using Newspoint.Infrastructure.Database.Seeders;

namespace Newspoint.Server.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<DbContext>();
        
        // Migrate database.
        await dbContext.Database.MigrateAsync();
        
        // Seed database if enabled in configuration.
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        var seedDb = app.Configuration.GetValue<bool>("Database:Seed");
        
        if (app.Environment.IsDevelopment() && seedDb)
            await seeder.Seed();
    }
}