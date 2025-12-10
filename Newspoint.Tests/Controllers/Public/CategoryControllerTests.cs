using AutoMapper;
using Moq;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.Controllers;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Tests.Controllers.Public;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CategoryController(_mockService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetCategories()
    {
        // Arrange
        var categories = new List<Category>();
        _mockService.Setup(a => a.GetAll())
            .ReturnsAsync(categories);

        _mockMapper.Setup(m => m.Map<IEnumerable<CategoryDto>>(categories))
            .Returns(new List<CategoryDto>());

        // Test
        var result = await _controller.GetAll();
        Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(result);

        _mockService.Verify(s => s.GetAll(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<CategoryDto>>(categories), Times.Once);
    }
}
