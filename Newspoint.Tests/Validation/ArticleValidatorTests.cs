using FluentValidation.TestHelper;
using Newspoint.Application.Validation;
using Newspoint.Domain.Entities;

namespace Newspoint.Tests.Validation;

public class ArticleValidatorTests
{
    private readonly ArticleValidator _validator = new();

    [Fact]
    public void Should_HaveValidationErrorFor_Title_When_Empty()
    {
        var article = new Article { Title = "", Content = "Some content", CategoryId = 1, AuthorId = 1 };

        var result = _validator.TestValidate(article);
        result.ShouldHaveValidationErrorFor(a => a.Title);
    }

    [Fact]
    public void Should_HaveValidationErrorFor_Content_When_Empty()
    {
        var article = new Article { Title = "Title", Content = "", CategoryId = 1, AuthorId = 1 };

        var result = _validator.TestValidate(article);
        result.ShouldHaveValidationErrorFor(a => a.Content);
    }

    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_Article_IsValid()
    {
        var article = new Article { Title = "Valid title", Content = "Valid content", CategoryId = 1, AuthorId = 1 };

        var result = _validator.TestValidate(article);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
