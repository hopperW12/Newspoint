using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface IArticleService : IService
{
    Task<ICollection<Article>> GetAll();
    Task<Result<Article>> GetById(int id);
    Task<Result<Article>> GetByIdWithComments(int id);
    Task<Result<Article>> Add(Article article);
    Task<Result<Article>> Update(Article article);
    Task<Result> Delete(int id);
}
public class ArticleService : IArticleService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;

    public ArticleService(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IArticleRepository articleRepository)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _articleRepository = articleRepository;
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
        // Find category in DB
        var category = await _categoryRepository.GetById(article.CategoryId);
        if (category == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.CategoryNotFound);

        article.Category = category;

        // Find author in DB
        var author = await _userRepository.GetById(article.AuthorId);
        if (author == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        // Check author permission
        if (author.Role == Role.Reader)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        article.Author = author;

        // Add article to DB
        var result = await _articleRepository.Add(article);
        if (result == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleError);

        return Result<Article>.Ok(result);
    }

    public async Task<Result<Article>> Update(Article article)
    {
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

        var result = await _articleRepository.Update(articleDb);
        if (result == null)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleError);

        return Result<Article>.Ok(result);
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _articleRepository.Delete(id);
        if (!result)
            return Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result.Ok();
    }
}