using AutoMapper;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.Controllers;
using Newspoint.Server.Areas.Public.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;

namespace Newspoint.Tests.Controllers.Public;

public class CommentControllerTests
{
    private readonly CommentController _controller;
    private readonly Mock<ICommentService> _mockService;
    private readonly Mock<IMapper> _mockMapper;

    public CommentControllerTests()
    {
        _mockService = new Mock<ICommentService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CommentController(
            _mockService.Object,
            _mockMapper.Object);
    }


    // Get Comment By Id

    [Fact]
    public async Task GetCommentById_When_ResultOk()
    {
        // Arrange
        _mockService.Setup(a => a.GetById(1))
            .ReturnsAsync(Result<Comment>.Ok(new Comment()));
        _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
            .Returns(new CommentDto());

        // Test
        var actionResult = await _controller.GetCommentById(1);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<CommentDto>>(okResult.Value);

        Assert.True(result.Success);
        Assert.IsType<CommentDto>(result.Data);
        _mockService.Verify(s => s.GetById(1), Times.Once);
    }

    [Fact]
    public async Task GetCommentById_When_ResultNotFound()
    {
        // Arrange
        _mockService.Setup(a => a.GetById(1))
            .ReturnsAsync(Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound));

        // Test
        var actionResult = await _controller.GetCommentById(1);
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<CommentDto>>(notFoundResult.Value);

        Assert.False(result.Success);
        _mockService.Verify(s => s.GetById(1), Times.Once);
    }

    [Fact]
    public async Task GetCommentById_When_ResultStatusCode()
    {
        // Arrange
        _mockService.Setup(a => a.GetById(1))
            .ReturnsAsync(Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError));

        // Test
        var actionResult = await _controller.GetCommentById(1);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        Assert.Equal(500, objectResult.StatusCode);
        _mockService.Verify(s => s.GetById(1), Times.Once);
    }
}
