using FluentValidation;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage(ServiceMessages.UserEmailRequired)
            .EmailAddress().WithMessage(ServiceMessages.UserEmailInvalid);

        RuleFor(u => u.FirstName)
            .NotEmpty().WithMessage(ServiceMessages.UserFirstNameRequired);

        RuleFor(u => u.LastName)
            .NotEmpty().WithMessage(ServiceMessages.UserLastNameRequired);

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage(ServiceMessages.UserPasswordRequired);
    }
}
