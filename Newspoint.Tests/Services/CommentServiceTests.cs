using FluentValidation;
using FluentValidation.Results;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Tests.Services;

public class CommentServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<ICommentRepository> _mockCommentRepository;
    private readonly Mock<IValidator<Comment>> _mockValidator;
    private readonly CommentService _service;

    public CommentServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockCommentRepository = new Mock<ICommentRepository>();
        _mockValidator = new Mock<IValidator<Comment>>();
        _service = new CommentService(
            _mockUserRepository.Object,
            _mockArticleRepository.Object,
            _mockCommentRepository.Object,
            _mockValidator.Object);
    }

    private static ValidationResult ValidResult() => new();

    private static ValidationResult InvalidResult(string error) => new([new ValidationFailure("prop", error)]);

    // GetById

    [Fact]
    public async Task GetById_When_CommentExists_ReturnsOk()
    {
        // Arrange
        var comment = new Comment { Id = 1 };
        _mockCommentRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(comment);

        // Test
        var result = await _service.GetById(1);

        Assert.True(result.Success);
        Assert.Equal(comment, result.Data);
    }

    [Fact]
    public async Task GetById_When_CommentNull_ReturnsNotFound()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetById(1))
            .ReturnsAsync((Comment?)null);

        // Test
        var result = await _service.GetById(1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    // Add

    [Fact]
    public async Task Add_When_ValidAndArticleAndAuthorOk_ReturnsOk()
    {
        // Arrange
        var comment = new Comment { Id = 1, ArticleId = 2, AuthorId = 3, Content = "c" };
        var article = new Article { Id = 2 };
        var author = new User { Id = 3 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockArticleRepository.Setup(r => r.GetById(comment.ArticleId))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(comment.AuthorId))
            .ReturnsAsync(author);

        _mockCommentRepository.Setup(r => r.Add(It.IsAny<Comment>()))
            .ReturnsAsync((Comment c) => c);

        // Test
        var result = await _service.Add(comment);

        Assert.True(result.Success);
        Assert.Equal(article, result.Data!.Article);
        Assert.Equal(author, result.Data!.Author);
        Assert.NotEqual(default, result.Data!.PublishedAt);
    }

    [Fact]
    public async Task Add_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var comment = new Comment { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Add(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
        _mockArticleRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Add_When_ArticleNotFound_ReturnsNotFound()
    {
        // Arrange
        var comment = new Comment { Id = 1, ArticleId = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockArticleRepository.Setup(r => r.GetById(comment.ArticleId))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.Add(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Add_When_AuthorNotFound_ReturnsNotFound()
    {
        // Arrange
        var comment = new Comment { Id = 1, ArticleId = 2, AuthorId = 3 };
        var article = new Article { Id = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockArticleRepository.Setup(r => r.GetById(comment.ArticleId))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(comment.AuthorId))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.Add(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Add_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var comment = new Comment { Id = 1, ArticleId = 2, AuthorId = 3 };
        var article = new Article { Id = 2 };
        var author = new User { Id = 3 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockArticleRepository.Setup(r => r.GetById(comment.ArticleId))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(comment.AuthorId))
            .ReturnsAsync(author);

        _mockCommentRepository.Setup(r => r.Add(It.IsAny<Comment>()))
            .ReturnsAsync((Comment?)null);

        // Test
        var result = await _service.Add(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Update

    [Fact]
    public async Task Update_When_ValidAndCommentExists_ReturnsOk()
    {
        // Arrange
        var commentDto = new Comment { Id = 1, Content = "new" };
        var existing = new Comment { Id = 1, Content = "old" };

        _mockValidator.Setup(v => v.ValidateAsync(commentDto, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCommentRepository.Setup(r => r.GetById(commentDto.Id))
            .ReturnsAsync(existing);

        _mockCommentRepository.Setup(r => r.Update(existing))
            .ReturnsAsync(existing);

        // Test
        var result = await _service.Update(commentDto);

        Assert.True(result.Success);
        Assert.Equal(commentDto.Content, existing.Content);
    }

    [Fact]
    public async Task Update_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var comment = new Comment { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Update(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_CommentNotFound_ReturnsNotFound()
    {
        // Arrange
        var comment = new Comment { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(comment, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCommentRepository.Setup(r => r.GetById(comment.Id))
            .ReturnsAsync((Comment?)null);

        // Test
        var result = await _service.Update(comment);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var commentDto = new Comment { Id = 1, Content = "new" };
        var existing = new Comment { Id = 1, Content = "old" };

        _mockValidator.Setup(v => v.ValidateAsync(commentDto, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCommentRepository.Setup(r => r.GetById(commentDto.Id))
            .ReturnsAsync(existing);

        _mockCommentRepository.Setup(r => r.Update(existing))
            .ReturnsAsync((Comment?)null);

        // Test
        var result = await _service.Update(commentDto);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Delete

    [Fact]
    public async Task Delete_When_RepositoryReturnsTrue_ReturnsOk()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.Delete(1))
            .ReturnsAsync(true);

        // Test
        var result = await _service.Delete(1);

        Assert.True(result.Success);
        _mockCommentRepository.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public async Task Delete_When_RepositoryReturnsFalse_ReturnsNotFound()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.Delete(1))
            .ReturnsAsync(false);

        // Test
        var result = await _service.Delete(1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    // CanUserDelete

    [Fact]
    public async Task CanUserDelete_When_CommentNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockCommentRepository.Setup(r => r.GetById(1))
            .ReturnsAsync((Comment?)null);

        // Test
        var result = await _service.CanUserDelete(5, 1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CanUserDelete_When_UserIsAdmin_ReturnsOk()
    {
        // Arrange
        var comment = new Comment { Id = 1, AuthorId = 2 };
        var user = new User { Id = 5, Role = Role.Admin };

        _mockCommentRepository.Setup(r => r.GetById(comment.Id))
            .ReturnsAsync(comment);

        _mockUserRepository.Setup(r => r.GetById(user.Id))
            .ReturnsAsync(user);

        // Test
        var result = await _service.CanUserDelete(user.Id, comment.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task CanUserDelete_When_UserIsAuthor_ReturnsOk()
    {
        // Arrange
        var comment = new Comment { Id = 1, AuthorId = 5 };
        var user = new User { Id = 5, Role = Role.Reader };

        _mockCommentRepository.Setup(r => r.GetById(comment.Id))
            .ReturnsAsync(comment);

        _mockUserRepository.Setup(r => r.GetById(user.Id))
            .ReturnsAsync(user);

        // Test
        var result = await _service.CanUserDelete(user.Id, comment.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task CanUserDelete_When_UserNullOrNoPermission_ReturnsUnknownError()
    {
        // Arrange
        var comment = new Comment { Id = 1, AuthorId = 2 };

        _mockCommentRepository.Setup(r => r.GetById(comment.Id))
            .ReturnsAsync(comment);

        _mockUserRepository.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.CanUserDelete(5, comment.Id);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }
}

