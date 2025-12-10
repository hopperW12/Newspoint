using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _service = new CategoryService(_mockCategoryRepository.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsCategoriesFromRepository()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "News" },
            new() { Id = 2, Name = "Tech" }
        };

        _mockCategoryRepository.Setup(r => r.GetAll())
            .ReturnsAsync(categories);

        // Test
        var result = await _service.GetAll();

        Assert.Equal(categories, result);
        _mockCategoryRepository.Verify(r => r.GetAll(), Times.Once);
    }
}

