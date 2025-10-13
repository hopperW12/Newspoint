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
    
    // Add Article
    
    [Fact]
    public async Task AddArticle_When_ReturnOk()
    {
        var dto = new ArticleDto { Id = 1, Title = "Test", Content = "Test" };
        _mockService.Setup(a => a.Add(dto))
            .ReturnsAsync(Result<ArticleDto>.Ok(dto));

        var actionResult = await _controller.AddArticle(dto);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);
        
        Assert.True(result.Success);
        Assert.Equal(dto, result.Data);
        _mockService.Verify(s => s.Add(dto), Times.Once);
    }
    
    [Fact]
    public async Task AddArticle_When_ReturnNotFound()
    {
        var dto = new ArticleDto { Id = 1, Title = "Test", Content = "Test" };
        _mockService.Setup(a => a.Add(dto))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        var actionResult = await _controller.AddArticle(dto);
        var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFoundObjectResult.Value);
        
        Assert.False(result.Success);
        _mockService.Verify(s => s.Add(dto), Times.Once);
    }
    
    [Fact]
    public async Task AddArticle_When_ReturnStatusCode()
    {
        var dto = new ArticleDto { Id = 1, Title = "Test", Content = "Test" };
        _mockService.Setup(a => a.Add(dto))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        var actionResult = await _controller.AddArticle(dto);
        var resultObject = Assert.IsType<ObjectResult>(actionResult);
        
        Assert.Equal(500, resultObject.StatusCode);
        _mockService.Verify(s => s.Add(dto), Times.Once);
    }

    // Update Article
    
    [Fact]
    public async Task UpdateArticle_When_ReturnOk()
    {
        var dto = new ArticleDto { Id = 1, Title = "Test", Content = "Test" };
        _mockService.Setup(a => a.Update(dto))
            .ReturnsAsync(Result<ArticleDto>.Ok(dto));

        var actionResult = await _controller.UpdateArticle(dto);
        var ok = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(ok.Value);

        Assert.True(result.Success);
        _mockService.Verify(s => s.Update(dto), Times.Once);
    }
    
    [Fact]
    public async Task UpdateArticle_When_ReturnNotFound()
    {
        var dto = new ArticleDto { Id = 42 };
        _mockService.Setup(a => a.Update(dto))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        var actionResult = await _controller.UpdateArticle(dto);
        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(notFound.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.Update(dto), Times.Once);
    }
    
    [Fact]
    public async Task UpdateArticle_When_ReturnStatusCode()
    {
        var dto = new ArticleDto { Id = 42 };
        _mockService.Setup(a => a.Update(dto))
            .ReturnsAsync(Result<ArticleDto>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        var actionResult = await _controller.UpdateArticle(dto);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        
        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.Update(dto), Times.Once);
    }

    // Delete Article
    
    [Fact]
    public async Task DeleteArticle_When_ReturnOk()
    {
        _mockService.Setup(a => a.Delete(10))
            .ReturnsAsync(Result.Ok());

        var action = await _controller.DeleteArticle(10);
        var ok = Assert.IsType<OkObjectResult>(action);
        var result = Assert.IsType<Result>(ok.Value);

        Assert.True(result.Success);
        _mockService.Verify(s => s.Delete(10), Times.Once);
    }

    [Fact]
    public async Task DeleteArticle_When_ReturnNotFound()
    {
        _mockService.Setup(a => a.Delete(11))
            .ReturnsAsync(Result.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));

        var action = await _controller.DeleteArticle(11);
        var notFound = Assert.IsType<NotFoundObjectResult>(action);
        var result = Assert.IsType<Result>(notFound.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.Delete(11), Times.Once);
    }
    
    [Fact]
    public async Task DeleteArticle_When_ReturnStatusCode()
    {
        _mockService.Setup(a => a.Delete(11))
            .ReturnsAsync(Result.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));

        var actionResult = await _controller.DeleteArticle(11);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        
        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.Delete(11), Times.Once);
    }
}