using Newspoint.Application.DTOs;
using Newspoint.Application.Mappers;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface IArticleService : IService
{
    Task<ICollection<ArticleDto>> GetAll();
    Task<Result<ArticleDto>> GetById(int id);
    Task<Result<ArticleDto>> GetByIdWithComments(int id);
    Task<Result<ArticleDto>> Add(ArticleDto articleDto);
    Task<Result<ArticleDto>> Update(ArticleDto articleDto);
    Task<Result> Delete(int id);
}
public class ArticleService : IArticleService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper<Article, ArticleDto> _articleMapper;
    private readonly IMapper<Comment, CommentDto> _commentMapper;

    public ArticleService(
        ICategoryRepository categoryRepository,
        IUserRepository userRepository,
        IArticleRepository articleRepository,
        IMapper<Article, ArticleDto> articleMapper,
        IMapper<Comment, CommentDto> commentMapper)
    {
        _categoryRepository = categoryRepository;
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _articleMapper = articleMapper;
        _commentMapper = commentMapper;
    }

    public async Task<ICollection<ArticleDto>> GetAll()
    {
        var articles = await _articleRepository.GetAll();
        return articles.Select(article => _articleMapper.Map(article)).ToList();
    }

    public async Task<Result<ArticleDto>> GetById(int id)
    {
        var article = await _articleRepository.GetById(id);
        if (article == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result<ArticleDto>.Ok(_articleMapper.Map(article));
    }

    public async Task<Result<ArticleDto>> GetByIdWithComments(int id)
    {
        var article = await _articleRepository.GetByIdWithComments(id);
        if (article == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        var articleDto = _articleMapper.Map(article);
        if (article.Comments.Count <= 0)
            return Result<ArticleDto>.Ok(articleDto);

        var comments = article.Comments
            .Select(comment => _commentMapper.Map(comment))
            .ToArray();
        articleDto.Comments = comments;

        return Result<ArticleDto>.Ok(articleDto);
    }

    public async Task<Result<ArticleDto>> Add(ArticleDto articleDto)
    {
        var article = _articleMapper.MapBack(articleDto);

        // Find category in DB
        var category = await _categoryRepository.GetById(article.CategoryId);
        if (category == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.CategoryNotFound);

        article.Category = category;

        // Find author in DB
        var author = await _userRepository.GetById(article.AuthorId);
        if (author == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        // Check author permission
        if (author.Role == Role.Reader)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        article.Author = author;

        // Add article to DB
        var result = await _articleRepository.Add(article);
        if (result == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleError);

        return Result<ArticleDto>.Ok(_articleMapper.Map(result));
    }

    public async Task<Result<ArticleDto>> Update(ArticleDto articleDto)
    {
        // Find category in DB
        var category = await _categoryRepository.GetById(articleDto.CategoryId);
        if (category == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.CategoryNotFound);

        // Update article in DB
        var article = await _articleRepository.GetById(articleDto.Id);
        if (article == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        article.Title = articleDto.Title;
        article.Content = articleDto.Content;
        article.CategoryId = category.Id;
        article.Category = category;

        var result = await _articleRepository.Update(article);
        if (result == null)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleError);

        return Result<ArticleDto>.Ok(_articleMapper.Map(result));
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _articleRepository.Delete(id);
        if (!result)
            return Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        return Result.Ok();
    }
}