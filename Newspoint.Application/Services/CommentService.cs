using FluentValidation;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUserRepository _userRepository;
    private readonly IArticleRepository _articleRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IValidator<Comment> _commentValidator;

    public CommentService(
        IUserRepository userRepository,
        IArticleRepository articleRepository,
        ICommentRepository commentRepository,
        IValidator<Comment> commentValidator)
    {
        _userRepository = userRepository;
        _articleRepository = articleRepository;
        _commentRepository = commentRepository;
        _commentValidator = commentValidator;
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
        // Validace obsahu komentáře.
        var validationResult = await _commentValidator.ValidateAsync(comment);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<Comment>.Error(ResultErrorType.Validation, firstError);
        }

        // Ověření, že komentovaný článek existuje.
        var article = await _articleRepository.GetById(comment.ArticleId);
        if (article == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound);

        comment.ArticleId = article.Id;
        comment.Article = article;

        // Ověření, že autor komentáře existuje.
        var author = await _userRepository.GetById(comment.AuthorId);
        if (author == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.AuthorNotFound);

        comment.AuthorId = author.Id;
        comment.Author = author;

        // Nastavení času publikace komentáře.
        comment.PublishedAt = DateTime.Now;

        // Uložení komentáře do databáze.
        var result = await _commentRepository.Add(comment);
        if (result == null)
            return Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError);

        return Result<Comment>.Ok(result);
    }

    public async Task<Result<Comment>> Update(Comment commentDto)
    {
        // Validace nového obsahu komentáře.
        var validationResult = await _commentValidator.ValidateAsync(commentDto);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<Comment>.Error(ResultErrorType.Validation, firstError);
        }

        // Načtení komentáře z databáze podle Id.
        var commentDb = await _commentRepository.GetById(commentDto.Id);
        if (commentDb == null)
            return Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        // Aktualizace pouze textu komentáře.
        commentDb.Content = commentDto.Content;

        // Uložení změn do databáze.
        var result = await _commentRepository.Update(commentDb);
        if (result == null)
            return Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError);

        return Result<Comment>.Ok(result);
    }

    public async Task<Result> Delete(int id)
    {
        var result = await _commentRepository.Delete(id);
        if (!result)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        return Result.Ok();
    }

    public Task<ICollection<Comment>> GetUserComments(int userId)
    {
        return _commentRepository.GetUserComments(userId);
    }

    public async Task<Result> CanUserDelete(int userId, int commentId)
    {
        // Ověření, že komentář existuje.
        var comment = await _commentRepository.GetById(commentId);
        if (comment == null)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound);

        // Kontrola, že uživatel je autor komentáře nebo admin.
        var user = await _userRepository.GetById(userId);
        if (user == null || !(user.Role == Role.Admin || user.Id == comment.AuthorId))
            return Result.Error(ResultErrorType.UnknownError, ServiceMessages.Error);

        return Result.Ok();
    }
}
