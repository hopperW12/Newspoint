using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.Controllers;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Interfaces;

namespace Newspoint.Tests.Controllers.Admin;

public class ArticleControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly Mock<IMapper<Article, ArticleDto>> _mockArticleMapper;
    private readonly Mock<IMapper<Article, ArticleCreateDto>> _mockArticleCreateMapper;
    private readonly Mock<IMapper<Article, ArticleUpdateDto>> _mockArticleUpdateMapper;
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _mockArticleMapper = new Mock<IMapper<Article, ArticleDto>>();
        _mockArticleCreateMapper = new Mock<IMapper<Article, ArticleCreateDto>>();
        _mockArticleUpdateMapper = new Mock<IMapper<Article, ArticleUpdateDto>>();
        _controller = new ArticleController(
            _mockService.Object,
            _mockArticleMapper.Object,
            _mockArticleCreateMapper.Object,
            _mockArticleUpdateMapper.Object);
    }

    // Add Article

    [Fact]
    public async Task AddArticle_When_ReturnOk()
    {
        // Arrange
        var article = new Article();
        var articleDto = new ArticleDto();
        var articleCreateDto = new ArticleCreateDto();

        _mockService.Setup(a => a.Add(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Ok(article));
        _mockArticleCreateMapper.Setup(m => m.MapBack(It.IsAny<ArticleCreateDto>()))
            .Returns(article);
        _mockArticleMapper.Setup(m => m.Map(It.IsAny<Article>()))
            .Returns(articleDto);

        // Test
        var actionResult = await _controller.AddArticle(articleCreateDto);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

        Assert.True(result.Success);
        Assert.Equal(articleDto, result.Data);
        _mockService.Verify(s => s.Add(article), Times.Once);
    }

    [Fact]
    public async Task AddArticle_When_ReturnNotFound()
    {
        // Arrange
        var article = new Article();
        var articleCreateDto = new ArticleCreateDto();

        _mockService.Setup(a => a.Add(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));
        _mockArticleCreateMapper.Setup(m => m.MapBack(It.IsAny<ArticleCreateDto>()))
            .Returns(article);

        // Test
        var actionResult = await _controller.AddArticle(articleCreateDto);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFoundResult.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.Add(article), Times.Once);
    }

    [Fact]
    public async Task AddArticle_When_ReturnStatusCode()
    {
        // Arrange
        var article = new Article();
        var articleCreateDto = new ArticleCreateDto();

        _mockService.Setup(a => a.Add(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));
        _mockArticleCreateMapper.Setup(m => m.MapBack(It.IsAny<ArticleCreateDto>()))
            .Returns(article);

        // Test
        var actionResult = await _controller.AddArticle(articleCreateDto);
        var resultObject = Assert.IsType<ObjectResult>(actionResult);

        Assert.Equal(500, resultObject.StatusCode);
        _mockService.Verify(s => s.Add(article), Times.Once);
    }

    // Update Article

    [Fact]
    public async Task UpdateArticle_When_ReturnOk()
    {
        // Arrange
        var article = new Article();
        var articleDto = new ArticleDto();
        var articleUpdateDto = new ArticleUpdateDto();

        _mockService.Setup(a => a.Update(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Ok(article));
        _mockArticleUpdateMapper.Setup(m => m.MapBack(It.IsAny<ArticleUpdateDto>()))
            .Returns(article);
        _mockArticleMapper.Setup(m => m.Map(It.IsAny<Article>()))
            .Returns(articleDto);

        // Test
        var actionResult = await _controller.UpdateArticle(articleUpdateDto);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

        Assert.True(result.Success);
        _mockService.Verify(s => s.Update(article), Times.Once);
    }

    [Fact]
    public async Task UpdateArticle_When_ReturnNotFound()
    {
        // Arrange
        var article = new Article();
        var articleUpdateDto = new ArticleUpdateDto();

        _mockService.Setup(a => a.Update(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));
        _mockArticleUpdateMapper.Setup(m => m.MapBack(It.IsAny<ArticleUpdateDto>()))
            .Returns(article);

        // Test
        var actionResult = await _controller.UpdateArticle(articleUpdateDto);
        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFound.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.Update(article), Times.Once);
    }

    [Fact]
    public async Task UpdateArticle_When_ReturnStatusCode()
    {
        // Arrange
        var article = new Article();
        var articleUpdateDto = new ArticleUpdateDto();

        _mockService.Setup(a => a.Update(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));
        _mockArticleUpdateMapper.Setup(m => m.MapBack(It.IsAny<ArticleUpdateDto>()))
            .Returns(article);

        // Test
        var actionResult = await _controller.UpdateArticle(articleUpdateDto);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);

        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.Update(article), Times.Once);
    }

    // Delete Article

    [Fact]
    public async Task DeleteArticle_When_ReturnOk()
    {
        _mockService.Setup(a => a.Delete(10))
            .ReturnsAsync(Result.Ok());

        var actionResult = await _controller.DeleteArticle(10);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result>(okResult.Value);

        Assert.True(result.Success);
        _mockService.Verify(s => s.Delete(10), Times.Once);
    }

    [Fact]
    public async Task DeleteArticle_When_ReturnNotFound()
    {
        // Arrange
        _mockService.Setup(a => a.Delete(11))
            .ReturnsAsync(Result.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        // Test
        var actionResult = await _controller.DeleteArticle(11);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result>(notFoundResult.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.Delete(11), Times.Once);
    }

    [Fact]
    public async Task DeleteArticle_When_ReturnStatusCode()
    {
        // Arrange
        _mockService.Setup(a => a.Delete(11))
            .ReturnsAsync(Result.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        // Test
        var actionResult = await _controller.DeleteArticle(11);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);

        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.Delete(11), Times.Once);
    }
}