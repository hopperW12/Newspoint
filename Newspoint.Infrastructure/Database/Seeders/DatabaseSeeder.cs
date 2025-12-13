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

        // Odstraníme duplicity v názvech kategorií
        categories = categories
            .GroupBy(c => c.Name)
            .Select(g => g.First())
            .ToList();

        // Vynecháme kategorie, které už v DB existují se stejným jménem
        var existingCategoryNames = _context.Categories
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        categories = categories
            .Where(c => !existingCategoryNames.Contains(c.Name))
            .ToList();

        await _context.Users.AddRangeAsync(users);

        if (categories.Count > 0)
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
