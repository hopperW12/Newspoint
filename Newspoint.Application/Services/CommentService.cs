using Newspoint.Application.DTOs;
using Newspoint.Application.Mappers;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface ICommentService : IService
{
    Task<Result<CommentDto>> GetById(int id);
    Task<Result<CommentDto>> Add(CommentDto commentDto);
    Task<Result<CommentDto>> Update(CommentDto commentDto);
    Task<Result> Delete(int id);
}

public class CommentService : ICommentService 
{
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IMapper<Comment, CommentDto> _commentMapper;

    public CommentService(
        IUserRepository userRepository,
        IArticleRepository articleRepository,
        ICommentRepository commentRepository,
        IMapper<Comment, CommentDto> commentMapper)
    {
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _commentRepository = commentRepository;
        _commentMapper = commentMapper;
    }

    public async Task<Result<CommentDto>> GetById(int id)
    {
        var comment = await _commentRepository.GetById(id);
        if (comment == null)
            return Result<CommentDto>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);
        
        return Result<CommentDto>.Ok(_commentMapper.Map(comment));
    }

    public async Task<Result<CommentDto>> Add(CommentDto commentDto)
    {
        var comment = _commentMapper.MapBack(commentDto);
        
        // Find article in DB
        var article = await _articleRepository.GetById(comment.ArticleId);
        if (article == null)
            return Result<CommentDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);
        
        comment.ArticleId = article.Id;
        comment.Article = article;
        
        // Find author in DB
        var author = await _userRepository.GetById(comment.AuthorId);
        if (author == null)
            return Result<CommentDto>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);
        
        comment.AuthorId = author.Id;
        comment.Author = author;
        
        // Add comment to DB
        var result = await _commentRepository.Add(comment);
        if (result == null)
            return Result<CommentDto>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError);
        
        return Result<CommentDto>.Ok(_commentMapper.Map(result));
    }

    public async Task<Result<CommentDto>> Update(CommentDto commentDto)
    {
        // Find comment in DB
        var comment = await _commentRepository.GetById(commentDto.Id);
        if (comment == null)
            return Result<CommentDto>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);
        
        comment.Content = commentDto.Content;
        
        // Update comment in DB
        var result = await _commentRepository.Update(comment);
        if (result == null)
            return Result<CommentDto>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);
        
        return Result<CommentDto>.Ok(_commentMapper.Map(result));
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _commentRepository.Delete(id);
        if (!result)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);
        
        return Result.Ok();
    }
}