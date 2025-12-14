using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Account.Controllers;
using Newspoint.Server.Areas.Account.DTOs;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Tests.Controllers.Account;

public class CommentControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ICommentService> _mockCommentService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CommentController _controller;

    public CommentControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockCommentService = new Mock<ICommentService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CommentController(_mockUserService.Object, _mockCommentService.Object, _mockMapper.Object);
    }

    private void SetUserEmailClaim(string email)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }));
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }

    // Get Comments

    [Fact]
    public async Task GetComments_When_NoClaim_ReturnsUnauthorized()
    {
        // Arrange - set empty user to prevent FindFirstValue null-arg
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Test
        var actionResult = await _controller.GetComments();

        Assert.IsType<UnauthorizedResult>(actionResult);
    }

    [Fact]
    public async Task GetComments_When_UserFound_ReturnsOk()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };
        var comments = new List<Comment>();
        var mapped = new List<CommentDto>();

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockCommentService.Setup(c => c.GetUserComments(user.Id))
            .ReturnsAsync(comments);

        _mockMapper.Setup(m => m.Map<IEnumerable<CommentDto>>(comments))
            .Returns(mapped);

        // Test
        var actionResult = await _controller.GetComments();
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.Equal(mapped, okResult.Value);
        _mockUserService.Verify(s => s.GetByEmail(email), Times.Once);
        _mockCommentService.Verify(c => c.GetUserComments(user.Id), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<CommentDto>>(comments), Times.Once);
    }

    [Fact]
    public async Task GetComments_When_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync((User?)null);
        
        // Test
        var actionResult = await _controller.GetComments();

        Assert.IsType<UnauthorizedResult>(actionResult);
        _mockUserService.Verify(s => s.GetByEmail(email), Times.Once);
    }

    // Add Comment

    [Fact]
    public async Task AddComment_When_ServiceOk_ReturnsOk()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };
        var create = new AccountCommentCreateDto { Content = "x" };
        var comment = new Comment { Id = 1, Content = "x", AuthorId = user.Id };
        var commentDto = new CommentDto { Id = 1, Content = "x" };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<AccountCommentCreateDto>()))
            .Returns(comment);

        _mockCommentService.Setup(c => c.Add(It.IsAny<Comment>()))
            .ReturnsAsync(Result<Comment>.Ok(comment));

        _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
            .Returns(commentDto);

        // Test
        var actionResult = await _controller.AddComment(create);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<CommentDto>>(okResult.Value);

        Assert.True(result.Success);
        Assert.Equal(commentDto, result.Data);
        _mockCommentService.Verify(c => c.Add(It.IsAny<Comment>()), Times.Once);
    }

    [Fact]
    public async Task AddComment_When_NoClaim_ReturnsUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };
        
        // Test
        var dto = new AccountCommentCreateDto { Content = "x" };

        var result = await _controller.AddComment(dto);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task AddComment_When_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);
        
        // Test
        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync((User?)null);

        var dto = new AccountCommentCreateDto { Content = "x" };

        var result = await _controller.AddComment(dto);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task AddComment_When_ServiceReturnsNotFound_ReturnsNotFound()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };
        var dto = new AccountCommentCreateDto { Content = "x" };
        var comment = new Comment { Id = 1, Content = "x", AuthorId = user.Id };
        
        // Setup
        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<AccountCommentCreateDto>()))
            .Returns(comment);

        _mockCommentService.Setup(c => c.Add(It.IsAny<Comment>()))
            .ReturnsAsync(Result<Comment>.Error(ResultErrorType.NotFound, "x"));

        var actionResult = await _controller.AddComment(dto);
        var notFound = Assert.IsType<NotFoundObjectResult>(actionResult);
        var result = Assert.IsType<Result<CommentDto>>(notFound.Value);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task AddComment_When_ServiceReturnsUnknownError_ReturnsObjectResult()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);
        
        var user = new User { Id = 3, Email = email };
        var dto = new AccountCommentCreateDto { Content = "x" };
        var comment = new Comment { Id = 1, Content = "x", AuthorId = user.Id };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<AccountCommentCreateDto>()))
            .Returns(comment);

        _mockCommentService.Setup(c => c.Add(It.IsAny<Comment>()))
            .ReturnsAsync(Result<Comment>.Error(ResultErrorType.UnknownError, "x"));

        // Test
        var actionResult = await _controller.AddComment(dto);
        var objectResult = Assert.IsType<ObjectResult>(actionResult);
        var result = Assert.IsType<Result<CommentDto>>(objectResult.Value);

        Assert.False(result.Success);
    }

    // Delete Comment

    [Fact]
    public async Task DeleteComment_When_NoClaim_ReturnsUnauthorized()
    {
        // Arrange - set empty user to prevent FindFirstValue null-arg
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Test
        var action = await _controller.DeleteComment(1);

        Assert.IsType<UnauthorizedResult>(action);
    }

    [Fact]
    public async Task DeleteComment_When_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _controller.DeleteComment(1);

        Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public async Task DeleteComment_When_CanUserDeleteNotFound_ReturnsNotFound()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockCommentService.Setup(c => c.CanUserDelete(user.Id, 5))
            .ReturnsAsync(Result.Error(ResultErrorType.NotFound, "x"));

        // Test
        var action = await _controller.DeleteComment(5);

        Assert.IsType<NotFoundObjectResult>(action);
        _mockCommentService.Verify(c => c.CanUserDelete(user.Id, 5), Times.Once);
    }

    [Fact]
    public async Task DeleteComment_When_CanUserDeleteUnknownError_ReturnsObjectResult()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };
        
        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockCommentService.Setup(c => c.CanUserDelete(user.Id, 5))
            .ReturnsAsync(Result.Error(ResultErrorType.UnknownError, "x"));
        
        // Test
        var action = await _controller.DeleteComment(5);

        var objectResult = Assert.IsType<ObjectResult>(action);
        var result = Assert.IsType<Result>(objectResult.Value);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task DeleteComment_When_DeleteOk_ReturnsOk()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };
        
        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockCommentService.Setup(c => c.CanUserDelete(user.Id, 5))
            .ReturnsAsync(Result.Ok());

        _mockCommentService.Setup(c => c.Delete(5))
            .ReturnsAsync(Result.Ok());
        
        // Test
        var action = await _controller.DeleteComment(5);
        var okResult = Assert.IsType<OkObjectResult>(action);
        var result = Assert.IsType<Result>(okResult.Value);

        Assert.True(result.Success);
    }

    [Fact]
    public async Task DeleteComment_When_DeleteUnknownError_ReturnsObjectResult()
    {   
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 3, Email = email };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockCommentService.Setup(c => c.CanUserDelete(user.Id, 5))
            .ReturnsAsync(Result.Ok());

        _mockCommentService.Setup(c => c.Delete(5))
            .ReturnsAsync(Result.Error(ResultErrorType.UnknownError, "x"));
        
        // Test
        var action = await _controller.DeleteComment(5);
        var objectResult = Assert.IsType<ObjectResult>(action);
        var result = Assert.IsType<Result>(objectResult.Value);

        Assert.False(result.Success);
    }
}
