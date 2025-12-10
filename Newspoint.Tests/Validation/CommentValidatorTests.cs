using FluentValidation.TestHelper;
using Newspoint.Application.Validation;
using Newspoint.Domain.Entities;

namespace Newspoint.Tests.Validation;

public class CommentValidatorTests
{
    private readonly CommentValidator _validator = new();

    [Fact]
    public void Should_HaveValidationErrorFor_Content_When_Empty()
    {
        var comment = new Comment { Content = string.Empty };

        var result = _validator.TestValidate(comment);
        result.ShouldHaveValidationErrorFor(c => c.Content);
    }

    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_Content_IsProvided()
    {
        var comment = new Comment { Content = "Some comment" };

        var result = _validator.TestValidate(comment);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
