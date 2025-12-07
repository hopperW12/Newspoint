using FluentValidation;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Validation;

public class ArticleValidator : AbstractValidator<Article>
{
    public ArticleValidator()
    {
        RuleFor(a => a.Title)
            .NotEmpty().WithMessage(ServiceMessages.ArticleTitleRequired);

        RuleFor(a => a.Content)
            .NotEmpty().WithMessage(ServiceMessages.ArticleContentRequired);
    }
}
