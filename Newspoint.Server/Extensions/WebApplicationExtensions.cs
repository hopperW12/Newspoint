using Microsoft.EntityFrameworkCore;

namespace Newspoint.Server.Extensions;

public static class WebApplicationExtensions
{
    public static async Task MigrateAsync(this IHost app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<DbContext>();

        await dbContext.Database.MigrateAsync();
    }
}