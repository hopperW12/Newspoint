using Moq;
using Newspoint.Application.DTOs;
using Newspoint.Application.Services;
using Newspoint.Server.Controllers;

namespace Newspoint.Tests.Controllers;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockService;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockService.Object);
    }

    [Fact]
    public async Task GetCategories()
    {
        _mockService.Setup(a => a.GetAll())
            .ReturnsAsync(new List<CategoryDto>());

        var result = await _controller.GetAll();
        Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(result);
        _mockService.Verify(s => s.GetAll(), Times.Once);
    }
}