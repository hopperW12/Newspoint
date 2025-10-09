using Microsoft.Extensions.Logging;

namespace Newspoint.Infrastructure.Database.Seeders;

public class DatabaseSeeder
{
    private readonly DataDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(DataDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Seed()
    {
        _logger.LogInformation("Seeding database...");
        
        var users = FakeEntityFactory.User().Generate(10);
        var categories = FakeEntityFactory.Category().Generate(10);
        
        await _context.Users.AddRangeAsync(users);
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
        
        var articles = FakeEntityFactory.Article(users, categories).Generate(25);
        
        await _context.Articles.AddRangeAsync(articles);
        await _context.SaveChangesAsync();
        
        var comments = FakeEntityFactory.Comment(users, articles).Generate(70);
        
        await _context.Comments.AddRangeAsync(comments);
        await _context.SaveChangesAsync();
    }
}