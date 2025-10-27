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
    public class CommentControllerTests
    {
        private readonly Mock<ICommentService> _mockService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CommentController _controller;

        public CommentControllerTests()
        {
            _mockService = new Mock<ICommentService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new CommentController(
                _mockService.Object,
                _mockMapper.Object);
        }

        // Add Comment

        [Fact]
        public async Task AddComment_When_ReturnOk()
        {
            // Arrange
            var comment = new Comment { Id = 1, Content = "New article!" };
            var commentDto = new CommentDto { Id = 1, Content = "New article!" };
            var createDto = new CommentCreateDto { Content = "New article!" };

            _mockService.Setup(s => s.Add(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Ok(comment));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(commentDto);
            _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<CommentCreateDto>()))
                .Returns(comment);

            // Test
            var actionResult = await _controller.AddComment(createDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<Result<CommentDto>>(okResult.Value);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(commentDto, result.Data);
            _mockService.Verify(s => s.Add(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task AddComment_When_ReturnNotFound()
        {
            // Arrange
            var createDto = new CommentCreateDto();

            _mockService.Setup(s => s.Add(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(new CommentDto());

            // Test
            var actionResult = await _controller.AddComment(createDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsType<Result<CommentDto>>(notFoundResult.Value);

            Assert.False(result.Success);
            _mockService.Verify(s => s.Add(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task AddComment_When_ReturnStatusCode()
        {
            // Arrange
            var createDto = new CommentCreateDto();

            _mockService.Setup(s => s.Add(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(new CommentDto());

            // Test
            var actionResult = await _controller.AddComment(createDto);
            var objectResult = Assert.IsType<ObjectResult>(actionResult);

            Assert.Equal(500, objectResult.StatusCode);
            _mockService.Verify(s => s.Add(It.IsAny<Comment>()), Times.Once);
        }

        // Update Comment

        [Fact]
        public async Task UpdateComment_When_ReturnOk()
        {
            // Arrange
            var comment = new Comment { Id = 1, Content = "Updated comment" };
            var commentDto = new CommentDto { Id = 1, Content = "Updated comment" };
            var updateDto = new CommentUpdateDto { Content = "Updated comment" };

            _mockService.Setup(s => s.Update(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Ok(comment));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(commentDto);
            _mockMapper.Setup(m => m.Map<Comment>(It.IsAny<CommentUpdateDto>()))
                .Returns(comment);

            // Test
            var actionResult = await _controller.UpdateComment(updateDto);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<Result<CommentDto>>(okResult.Value);

            Assert.True(result.Success);
            Assert.Equal(commentDto, result.Data);
            _mockService.Verify(s => s.Update(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_When_ReturnNotFound()
        {
            // Arrange
            var updateDto = new CommentUpdateDto();

            _mockService.Setup(s => s.Update(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(new CommentDto());

            // Test
            var actionResult = await _controller.UpdateComment(updateDto);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsType<Result<CommentDto>>(notFoundResult.Value);

            Assert.False(result.Success);
            _mockService.Verify(s => s.Update(It.IsAny<Comment>()), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_When_ReturnStatusCode()
        {
            // Arrange
            var updateDto = new CommentUpdateDto();

            _mockService.Setup(s => s.Update(It.IsAny<Comment>()))
                .ReturnsAsync(Result<Comment>.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError));
            _mockMapper.Setup(m => m.Map<CommentDto>(It.IsAny<Comment>()))
                .Returns(new CommentDto());

            // Test
            var actionResult = await _controller.UpdateComment(updateDto);
            var objectResult = Assert.IsType<ObjectResult>(actionResult);

            Assert.Equal(500, objectResult.StatusCode);
            _mockService.Verify(s => s.Update(It.IsAny<Comment>()), Times.Once);
        }

        // Delete Comment

        [Fact]
        public async Task DeleteComment_When_ReturnOk()
        {
            // Arrange
            _mockService.Setup(s => s.Delete(5))
                .ReturnsAsync(Result.Ok());

            // Test
            var actionResult = await _controller.DeleteComment(5);
            var okResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsType<Result>(okResult.Value);

            Assert.True(result.Success);
            _mockService.Verify(s => s.Delete(5), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_When_ReturnNotFound()
        {
            // Arrange
            _mockService.Setup(s => s.Delete(6))
                .ReturnsAsync(Result.Error(ResultErrorType.NotFound, ServiceMessages.CommentNotFound));

            // Test
            var actionResult = await _controller.DeleteComment(6);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsType<Result>(notFoundResult.Value);

            Assert.False(result.Success);
            _mockService.Verify(s => s.Delete(6), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_When_ReturnStatusCode()
        {
            // Arrange
            _mockService.Setup(s => s.Delete(7))
                .ReturnsAsync(Result.Error(ResultErrorType.UnknownError, ServiceMessages.CommentError));

            // Test
            var actionResult = await _controller.DeleteComment(7);
            var objectResult = Assert.IsType<ObjectResult>(actionResult);

            Assert.Equal(500, objectResult.StatusCode);
            _mockService.Verify(s => s.Delete(7), Times.Once);
        }
    }
}
