using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface ICommentService : IService
{
    Task<Result<Comment>> GetById(int id);
    Task<Result<Comment>> Add(Comment comment);
    Task<Result<Comment>> Update(Comment comment);
    Task<Result> Delete(int id);
}

public class CommentService : ICommentService
{
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ICommentRepository _commentRepository;

    public CommentService(
        IUserRepository userRepository,
        IArticleRepository articleRepository,
        ICommentRepository commentRepository)
    {
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _commentRepository = commentRepository;
    }

    public async Task<Result<Comment>> GetById(int id)
    {
        var comment = await _commentRepository.GetById(id);
        if (comment == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        return Result<Comment>.Ok(comment);
    }

    public async Task<Result<Comment>> Add(Comment comment)
    {

        // Find article in DB
        var article = await _articleRepository.GetById(comment.ArticleId);
        if (article == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        comment.ArticleId = article.Id;
        comment.Article = article;

        // Find author in DB
        var author = await _userRepository.GetById(comment.AuthorId);
        if (author == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        comment.AuthorId = author.Id;
        comment.Author = author;
        
        // Set published date
        comment.PublishedAt = DateTime.Now;

        // Add comment to DB
        var result = await _commentRepository.Add(comment);
        if (result == null)
            return Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError);

        return Result<Comment>.Ok(result);
    }

    public async Task<Result<Comment>> Update(Comment commentDto)
    {
        // Find comment in DB
        var commentDb = await _commentRepository.GetById(commentDto.Id);
        if (commentDb == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        commentDb.Content = commentDto.Content;

        // Update comment in DB
        var result = await _commentRepository.Update(commentDb);
        if (result == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        return Result<Comment>.Ok(result);
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _commentRepository.Delete(id);
        if (!result)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        return Result.Ok();
    }
}