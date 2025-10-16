using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.DTOs;
using Newspoint.Application.Services;
using Newspoint.Server.Controllers;

namespace Newspoint.Tests.Controllers;

public class ArticleControllerTests
{
    private readonly Mock<IArticleService> _mockService;
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        _mockService = new Mock<IArticleService>();
        _controller = new ArticleController(_mockService.Object);
    }

    // Get Articles

    [Fact]
    public async Task GetArticles()
    {
        _mockService.Setup(a => a.GetAll())
            .ReturnsAsync(new List<ArticleDto>());

        var result = await _controller.GetArticles();
        Assert.IsAssignableFrom<IEnumerable<ArticleDto>>(result);
        _mockService.Verify(s => s.GetAll(), Times.Once);
    }

    // Get Article By Id

    [Fact]
    public async Task GetArticleById_When_ResultOk()
    {
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<ArticleDto>.Ok(new ArticleDto()));

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
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        var actionResult = await _controller.GetById(1);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFoundResult.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.GetByIdWithComments(1), Times.Once);
    }

    [Fact]
    public async Task GetArticleById_When_ResultStatusCode()
    {
        _mockService.Setup(a => a.GetByIdWithComments(1))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        var actionResult = await _controller.GetById(1);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.GetByIdWithComments(1), Times.Once);
    }
}