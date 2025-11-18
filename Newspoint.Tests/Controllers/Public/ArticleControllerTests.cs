using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.Controllers;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Tests.Controllers.Public;

public class ArticleControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ArticleController(
            _mockService.Object,
            _mockMapper.Object);
    }

    // Get Articles

    [Fact]
    public async Task GetArticles()
    {
        // Arrange
        _mockService.Setup(a => a.GetAll())
            .ReturnsAsync(new List<Article>());

        // Test
        var result = await _controller.GetArticles();
        Assert.IsAssignableFrom<IEnumerable<ArticleDto>>(result);
        _mockService.Verify(s => s.GetAll(), Times.Once);
    }

    // Get Article By Id

    [Fact]
    public async Task GetArticleById_When_ResultOk()
    {
        // Arrange
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<Article>.Ok(new Article()));
        _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
            .Returns(new ArticleDto());

        // Test
        var actionResult = await _controller.GetById(1);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

        Assert.True(result.Success);
        Assert.IsType<ArticleDto>(result.Data);
        _mockService.Verify(s => s.GetByIdWithComments(1), Times.Once);
    }

    [Fact]
    public async Task GetArticleById_When_ResultNotFound()
    {
        // Arrange
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        // Test
        var actionResult = await _controller.GetById(1);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFoundResult.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.GetByIdWithComments(1), Times.Once);
    }

    [Fact]
    public async Task GetArticleById_When_ResultStatusCode()
    {
        // Arrange
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        // Test
        var actionResult = await _controller.GetById(1);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.GetByIdWithComments(1), Times.Once);
    }
}