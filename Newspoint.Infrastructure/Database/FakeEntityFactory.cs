using Bogus;
using Newspoint.Domain;
using Newspoint.Domain.Entities;

namespace Newspoint.Infrastructure.Database;

public static class FakeEntityFactory
{
    public static Faker<User> User()
    {
        return new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => PasswordHasher.HashPassword(f.Internet.Password()))
            .RuleFor(u => u.Role, f => f.PickRandom<Role>());
    }

    public static Faker<Category> Category()
    {
        return new Faker<Category>()
            .RuleFor(c => c.Name, f => f.Commerce.Categories(1).First());
    }

    public static Faker<Article> Article(ICollection<User> users, ICollection<Category> categories)
    {
        return new Faker<Article>()
            .RuleFor(a => a.Title, f => f.Lorem.Sentence(4))
            .RuleFor(a => a.Content, f => f.Lorem.Paragraphs(2, 5))
            .RuleFor(a => a.PublishedAt, f => f.Date.Past())
            .RuleFor(a => a.AuthorId, f => f.PickRandom(users).Id)
            .RuleFor(a => a.CategoryId, f => f.PickRandom(categories).Id);
    }

    public static Faker<Comment> Comment(ICollection<User> users, ICollection<Article> articles)
    {
        return new Faker<Comment>()
            .RuleFor(c => c.Content, f => f.Lorem.Sentence(25))
            .RuleFor(c => c.PublishedAt, f => f.Date.Past())
            .RuleFor(c => c.ArticleId, f => f.PickRandom(articles).Id)
            .RuleFor(c => c.AuthorId, f => f.PickRandom(users).Id);
    }
}
