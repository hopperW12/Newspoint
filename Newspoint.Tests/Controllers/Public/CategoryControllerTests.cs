using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Controllers;

namespace Newspoint.Tests.Controllers.Public;

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
        // Arrange
        _mockService.Setup(a => a.GetAll())
            .ReturnsAsync(new List<Category>());

        // Test
        var result = await _controller.GetAll();
        Assert.IsAssignableFrom<IEnumerable<Category>>(result);
        _mockService.Verify(s => s.GetAll(), Times.Once);
    }
}