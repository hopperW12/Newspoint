using FluentValidation;
using FluentValidation.Results;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Tests.Services;

public class ArticleServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IArticleRepository> _mockArticleRepository;
    private readonly Mock<IValidator<Article>> _mockValidator;
    private readonly ArticleService _service;

    public ArticleServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockArticleRepository = new Mock<IArticleRepository>();
        _mockValidator = new Mock<IValidator<Article>>();
        _service = new ArticleService(
            _mockCategoryRepository.Object,
            _mockUserRepository.Object,
            _mockArticleRepository.Object,
            _mockValidator.Object);
    }

    private static ValidationResult ValidResult() => new ();

    private static ValidationResult InvalidResult(string error) => new ([new ValidationFailure("prop", error)]);

    // GetById

    [Fact]
    public async Task GetById_When_ArticleExists_ReturnsOk()
    {
        // Arrange
        var article = new Article { Id = 1 };
        _mockArticleRepository.Setup(r => r.GetById(1))
            .ReturnsAsync(article);

        // Test
        var result = await _service.GetById(1);

        Assert.True(result.Success);
        Assert.Equal(article, result.Data);
    }

    [Fact]
    public async Task GetById_When_ArticleNull_ReturnsNotFound()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetById(1))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.GetById(1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    // Add

    [Fact]
    public async Task Add_When_ValidAndCategoryAndAuthorOk_ReturnsOk()
    {
        // Arrange
        var article = new Article { Id = 1, Title = "t", Content = "c", CategoryId = 2, AuthorId = 3, Author = new User { Id = 3, Role = Role.Editor } };
        var category = new Category { Id = 2 };
        var author = new User { Id = 3, Role = Role.Editor };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockUserRepository.Setup(r => r.GetById(article.AuthorId))
            .ReturnsAsync(author);

        _mockArticleRepository.Setup(r => r.Add(It.IsAny<Article>()))
            .ReturnsAsync((Article a) => a);

        // Test
        var result = await _service.Add(article);

        Assert.True(result.Success);
        Assert.Equal(category, result.Data!.Category);
        Assert.Equal(author, result.Data!.Author);
        Assert.NotEqual(default, result.Data!.PublishedAt);
    }

    [Fact]
    public async Task Add_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var article = new Article { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Add(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
        _mockCategoryRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(r => r.GetById(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Add_When_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync((Category?)null);

        // Test
        var result = await _service.Add(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Add_When_AuthorNotFound_ReturnsNotFound()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2, AuthorId = 3 };
        var category = new Category { Id = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockUserRepository.Setup(r => r.GetById(article.AuthorId))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.Add(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Add_When_AuthorIsReader_ReturnsValidationError()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2, AuthorId = 3 };
        var category = new Category { Id = 2 };
        var author = new User { Id = 3, Role = Role.Reader };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockUserRepository.Setup(r => r.GetById(article.AuthorId))
            .ReturnsAsync(author);

        // Test
        var result = await _service.Add(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task Add_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2, AuthorId = 3 };
        var category = new Category { Id = 2 };
        var author = new User { Id = 3, Role = Role.Editor };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockUserRepository.Setup(r => r.GetById(article.AuthorId))
            .ReturnsAsync(author);

        _mockArticleRepository.Setup(r => r.Add(It.IsAny<Article>()))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.Add(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Update

    [Fact]
    public async Task Update_When_ValidCategoryAndArticleExist_ReturnsOk()
    {
        // Arrange
        var article = new Article { Id = 1, Title = "New", Content = "C", CategoryId = 2, ImagePath = "img" };
        var category = new Category { Id = 2 };
        var existing = new Article { Id = 1, Title = "Old", Content = "Old", CategoryId = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync(existing);

        _mockArticleRepository.Setup(r => r.Update(existing))
            .ReturnsAsync(existing);

        // Test
        var result = await _service.Update(article);

        Assert.True(result.Success);
        Assert.Equal(article.Title, existing.Title);
        Assert.Equal(article.Content, existing.Content);
        Assert.Equal(category, existing.Category);
        Assert.Equal(article.ImagePath, existing.ImagePath);
    }

    [Fact]
    public async Task Update_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var article = new Article { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Update(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync((Category?)null);

        // Test
        var result = await _service.Update(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_ArticleNotFound_ReturnsNotFound()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2 };
        var category = new Category { Id = 2 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.Update(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var article = new Article { Id = 1, CategoryId = 2 };
        var category = new Category { Id = 2 };
        var existing = new Article { Id = 1 };

        _mockValidator.Setup(v => v.ValidateAsync(article, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockCategoryRepository.Setup(r => r.GetById(article.CategoryId))
            .ReturnsAsync(category);

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync(existing);

        _mockArticleRepository.Setup(r => r.Update(existing))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.Update(article);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Delete

    [Fact]
    public async Task Delete_When_RepositoryReturnsTrue_ReturnsOk()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.Delete(1))
            .ReturnsAsync(true);

        // Test
        var result = await _service.Delete(1);

        Assert.True(result.Success);
        _mockArticleRepository.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public async Task Delete_When_RepositoryReturnsFalse_ReturnsNotFound()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.Delete(1))
            .ReturnsAsync(false);

        // Test
        var result = await _service.Delete(1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    // CanUserDelete

    [Fact]
    public async Task CanUserDelete_When_ArticleNotFound_ReturnsNotFound()
    {
        // Arrange
        _mockArticleRepository.Setup(r => r.GetById(1))
            .ReturnsAsync((Article?)null);

        // Test
        var result = await _service.CanUserDelete(5, 1);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CanUserDelete_When_UserIsAdmin_ReturnsOk()
    {
        // Arrange
        var article = new Article { Id = 1, AuthorId = 2 };
        var user = new User { Id = 5, Role = Role.Admin };

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(user.Id))
            .ReturnsAsync(user);

        // Test
        var result = await _service.CanUserDelete(user.Id, article.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task CanUserDelete_When_UserIsAuthor_ReturnsOk()
    {
        // Arrange
        var article = new Article { Id = 1, AuthorId = 5 };
        var user = new User { Id = 5, Role = Role.Reader };

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(user.Id))
            .ReturnsAsync(user);

        // Test
        var result = await _service.CanUserDelete(user.Id, article.Id);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task CanUserDelete_When_UserNullOrNoPermission_ReturnsUnknownError()
    {
        // Arrange
        var article = new Article { Id = 1, AuthorId = 2 };

        _mockArticleRepository.Setup(r => r.GetById(article.Id))
            .ReturnsAsync(article);

        _mockUserRepository.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.CanUserDelete(5, article.Id);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }
}

