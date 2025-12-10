using FluentValidation.TestHelper;
using Newspoint.Application.Validation;
using Newspoint.Domain.Entities;

namespace Newspoint.Tests.Validation;

public class UserValidatorTests
{
    private readonly UserValidator _validator = new();

    [Fact]
    public void Should_HaveValidationErrors_For_AllRequiredFields_When_Missing()
    {
        var user = new User
        {
            Email = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Password = string.Empty
        };

        var result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(u => u.Email);
        result.ShouldHaveValidationErrorFor(u => u.FirstName);
        result.ShouldHaveValidationErrorFor(u => u.LastName);
        result.ShouldHaveValidationErrorFor(u => u.Password);
    }

    [Fact]
    public void Should_HaveValidationErrorFor_Email_When_InvalidFormat()
    {
        var user = new User
        {
            Email = "not-an-email",
            FirstName = "A",
            LastName = "B",
            Password = "P@ssw0rd"
        };

        var result = _validator.TestValidate(user);

        result.ShouldHaveValidationErrorFor(u => u.Email);
    }

    [Fact]
    public void Should_NotHaveAnyValidationErrors_When_UserIsValid()
    {
        var user = new User
        {
            Email = "user@test.com",
            FirstName = "A",
            LastName = "B",
            Password = "P@ssw0rd"
        };

        var result = _validator.TestValidate(user);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
