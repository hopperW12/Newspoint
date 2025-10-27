using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.Controllers;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Tests.Controllers.Admin
{
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

        // Add Article

        [Fact]
        public async Task AddArticle_When_ReturnOk()
        {
            // Arrange
            var articleCreateDto = new ArticleCreateDto();
            var article = new Article { Id = 1, Title = "Test Article" };
            var articleDto = new ArticleDto { Id = 1, Title = "Test Article" };

            _mockService.Setup(a => a.Add(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Ok(article));
            _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(articleDto);
            _mockMapper.Setup(m => m.Map<Article>(It.IsAny<ArticleCreateDto>()))
                .Returns(article);

            // Test
            var actionResult = await _controller.AddArticle(articleCreateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

            Assert.True(result.Success);
            Assert.Equal(articleDto, result.Data);
            _mockService.Verify(s => s.Add(It.IsAny<Article>()), Times.Once);
        }

        [Fact]
        public async Task AddArticle_When_ReturnNotFound()
        {
            // Arrange
            var articleCreateDto = new ArticleCreateDto();
            var article = new Article();

            _mockService.Setup(a => a.Add(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));
            _mockMapper.Setup(m => m.Map<Article>(It.IsAny<ArticleCreateDto>()))
                .Returns(article);

            // Test
            var actionResult = await _controller.AddArticle(articleCreateDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsType<Result<ArticleDto>>(notFoundResult.Value);

            Assert.False(result.Success);
            _mockService.Verify(s => s.Add(It.IsAny<Article>()), Times.Once);
        }

        [Fact]
        public async Task AddArticle_When_ReturnStatusCode()
        {
            // Arrange
            var articleCreateDto = new ArticleCreateDto();
            var article = new Article();

            _mockService.Setup(a => a.Add(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));
            _mockMapper.Setup(m => m.Map<Article>(It.IsAny<ArticleCreateDto>()))
                .Returns(article);

            // Test
            var actionResult = await _controller.AddArticle(articleCreateDto);
            var resultObject = Assert.IsType<ObjectResult>(actionResult);

            Assert.Equal(500, resultObject.StatusCode);
            _mockService.Verify(s => s.Add(It.IsAny<Article>()), Times.Once);
        }

        // Update Article

        [Fact]
        public async Task UpdateArticle_When_ReturnOk()
        {
            // Arrange
            var articleUpdateDto = new ArticleUpdateDto { Title = "Updated Title" };
            var article = new Article { Id = 1, Title = "Updated Title" };
            var articleDto = new ArticleDto { Id = 1, Title = "Updated Title" };

            _mockService.Setup(a => a.Update(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Ok(article));
            _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(articleDto);
            _mockMapper.Setup(m => m.Map<Article>(It.IsAny<ArticleUpdateDto>()))
                .Returns(article);

            // Test
            var actionResult = await _controller.UpdateArticle(articleUpdateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<Result<ArticleDto>>(okResult.Value);

            Assert.True(result.Success);
            Assert.Equal(articleDto, result.Data);
            _mockService.Verify(s => s.Update(It.IsAny<Article>()), Times.Once);
        }

        [Fact]
        public async Task UpdateArticle_When_ReturnNotFound()
        {
            // Arrange
            var articleUpdateDto = new ArticleUpdateDto();

            _mockService.Setup(a => a.Update(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Error(ResultErrorType.NotFound, ServiceMessages.ArticleNotFound));
            _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(new ArticleDto());

            // Test
            var actionResult = await _controller.UpdateArticle(articleUpdateDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsType<Result<ArticleDto>>(notFoundResult.Value);

            Assert.False(result.Success);
            _mockService.Verify(s => s.Update(It.IsAny<Article>()), Times.Once);
        }

        [Fact]
        public async Task UpdateArticle_When_ReturnStatusCode()
        {
            // Arrange
            var articleUpdateDto = new ArticleUpdateDto();

            _mockService.Setup(a => a.Update(It.IsAny<Article>()))
                .ReturnsAsync(Result<Article>.Error(ResultErrorType.UnknownError, ServiceMessages.ArticleError));
            _mockMapper.Setup(m => m.Map<ArticleDto>(It.IsAny<Article>()))
                .Returns(new ArticleDto());

            // Test
            var actionResult = await _controller.UpdateArticle(articleUpdateDto);
            var objectResult = Assert.IsType<ObjectResult>(actionResult);

            Assert.Equal(500, objectResult.StatusCode);
            _mockService.Verify(s => s.Update(It.IsAny<Article>()), Times.Once);
        }

        // Delete Article

        [Fact]
        public async Task DeleteArticle_When_ReturnOk()
        {
            // Arrange
            _mockService.Setup(a => a.Delete(10))
                .ReturnsAsync(Result.Ok());

            // Test
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
}
