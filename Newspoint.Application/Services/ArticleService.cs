using FluentValidation;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Application.Services;

public class ArticleService : IArticleService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IValidator<Article> _articleValidator;

    public ArticleService(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IArticleRepository articleRepository,
        IValidator<Article> articleValidator)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _articleValidator = articleValidator;
    }

    public Task<ICollection<Article>> GetAll()
    {
        return _articleRepository.GetAll();
    }

    public async Task<Result<Article>> GetById(int id)
    {
        var article = await _articleRepository.GetById(id);
        if (article == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result<Article>.Ok(article);
    }

    public async Task<Result<Article>> GetByIdWithComments(int id)
    {
        var article = await _articleRepository.GetByIdWithComments(id);
        if (article == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result<Article>.Ok(article);
    }

    public async Task<Result<Article>> Add(Article article)
    {
        // FluentValidation
        var validationResult = await _articleValidator.ValidateAsync(article);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<Article>.Error(ResultErrorType.Validation, firstError);
        }

        // Find category in DB
        var category = await _categoryRepository.GetById(article.CategoryId);
        if (category == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.CategoryNotFound);

        article.Category = category;

        // Find author in DB
        var author = await _userRepository.GetById(article.AuthorId);
        if (author == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        article.Author = author;
        article.PublishedAt = DateTime.Now;

        // Add article to DB
        var result = await _articleRepository.Add(article);
        if (result == null)
            return Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError);

        return Result<Article>.Ok(result);
    }

    public async Task<Result<Article>> Update(Article article)
    {
        // FluentValidation
        var validationResult = await _articleValidator.ValidateAsync(article);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<Article>.Error(ResultErrorType.Validation, firstError);
        }

        // Find category in DB
        var category = await _categoryRepository.GetById(article.CategoryId);
        if (category == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.CategoryNotFound);

        // Update article in DB
        var articleDb = await _articleRepository.GetById(article.Id);
        if (articleDb == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        articleDb.Title = article.Title;
        articleDb.Content = article.Content;
        articleDb.CategoryId = category.Id;
        articleDb.Category = category;
        articleDb.ImagePath = article.ImagePath;

        var result = await _articleRepository.Update(articleDb);
        if (result == null)
            return Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError);

        return Result<Article>.Ok(result);
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _articleRepository.Delete(id);
        if (!result)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result.Ok();
    }

    public Task<ICollection<Article>> GetUserArticles(int userId)
    {
        return _articleRepository.GetUserArticles(userId);
    }

    public async Task<Result> CanUserEdit(int userId, int articleId)
    {
        var article = await _articleRepository.GetById(articleId);
        if (article == null)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        var user = await _userRepository.GetById(userId);
        if (user == null || !(user.Role == Role.Admin || user.Id == article.AuthorId))
            return Result.Error(ResultErrorType.UnknownError, ServiceMessages.Error);

        return Result.Ok();
    }
}
