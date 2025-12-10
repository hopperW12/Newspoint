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

public class ArticleControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IArticleService> _mockArticleService;
    private readonly Mock<IArticleImageService> _mockImageService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockArticleService = new Mock<IArticleService>();
        _mockImageService = new Mock<IArticleImageService>();
        _mockMapper = new Mock<IMapper>();

        _controller = new ArticleController(
            _mockUserService.Object,
            _mockArticleService.Object,
            _mockImageService.Object,
            _mockMapper.Object);
    }

    private void SetUserEmailClaim(string email)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }));
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
    }

    // Get Articles

    [Fact]
    public async Task GetArticles_When_NoEmailClaim_ReturnsUnauthorized()
    {
        // Arrange - set empty user (prevents FindFirstValue null-arg)
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        // Test
        var actionResult = await _controller.GetArticles();

        Assert.IsType<UnauthorizedResult>(actionResult);
    }

    [Fact]
    public async Task GetArticles_When_UserFound_ReturnsOk()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 2, Email = email };
        var articles = new List<Article>();
        var mapped = new List<ArticleDto>();

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockArticleService.Setup(s => s.GetUserArticles(user.Id))
            .ReturnsAsync(articles);

        _mockMapper.Setup(m => m.Map<IEnumerable<ArticleDto>>(articles))
            .Returns(mapped);

        // Test
        var actionResult = await _controller.GetArticles();
        var okResult = Assert.IsType<OkObjectResult>(actionResult);

        Assert.Equal(mapped, okResult.Value);
        _mockUserService.Verify(s => s.GetByEmail(email), Times.Once);
        _mockArticleService.Verify(s => s.GetUserArticles(user.Id), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<ArticleDto>>(articles), Times.Once);
    }

    // Add Article

    [Fact]
    public async Task AddArticle_When_ImageProvidedAndServiceOk_ReturnsOk()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 2, Email = email };
        var createDto = new AccountArticleCreateDto { Title = "t", Content = "c" };
        var article = new Article { Id = 1, AuthorId = user.Id };
        var articleDto = new ArticleDto { Id = 1, Title = "t", Content = "c" };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Article>(It.IsAny<AccountArticleCreateDto>()))
            .Returns(article);

        _mockImageService.Setup(i => i.SaveImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ReturnsAsync("/img/p.jpg");

        _mockArticleService.Setup(a => a.Add(It.IsAny<Article>()))
            .ReturnsAsync(Result<Article>.Ok(article));

        _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
            .Returns(articleDto);

        var fileMock = new Mock<IFormFile>();
        var content = "Hello World from a fake file";
        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.FileName).Returns("pic.jpg");
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

        // Test
        var actionResult = await _controller.AddArticle(createDto, fileMock.Object);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

        Assert.True(result.Success);
        Assert.Equal(articleDto, result.Data);
        _mockImageService.Verify(i => i.SaveImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
        _mockArticleService.Verify(a => a.Add(It.IsAny<Article>()), Times.Once);
    }

    [Fact]
    public async Task AddArticle_When_ImageServiceThrows_ReturnsBadRequest()
    {
        // Arrange
        var email = "u@u.com";
        SetUserEmailClaim(email);

        var user = new User { Id = 2, Email = email };
        var createDto = new AccountArticleCreateDto { Title = "t" };
        var article = new Article { Id = 1, AuthorId = user.Id };

        _mockUserService.Setup(s => s.GetByEmail(email))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<Article>(It.IsAny<AccountArticleCreateDto>()))
            .Returns(article);

        _mockImageService.Setup(i => i.SaveImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
            .ThrowsAsync(new InvalidOperationException("bad"));

        var fileMock = new Mock<IFormFile>();
        var content = "data";
        using var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

        fileMock.Setup(f => f.Length).Returns(ms.Length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(ms);

        // Test
        var actionResult = await _controller.AddArticle(createDto, fileMock.Object);
        var badRequest = Assert.IsType<BadRequestObjectResult>(actionResult);

        Assert.NotNull(badRequest.Value);
        _mockImageService.Verify(i => i.SaveImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
        _mockArticleService.Verify(a => a.Add(It.IsAny<Article>()), Times.Never);
    }
}
