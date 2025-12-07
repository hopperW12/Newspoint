using FluentValidation;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Validation;

public class CommentValidator : AbstractValidator<Comment>
{
    public CommentValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty().WithMessage(ServiceMessages.CommentContentRequired);
    }
}
