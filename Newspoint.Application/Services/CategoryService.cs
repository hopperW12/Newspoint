using Newspoint.Application.DTOs;
using Newspoint.Application.Mappers;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface ICategoryService : IService
{
    Task<ICollection<CategoryDto>> GetAll();
}

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper<Category, CategoryDto> _categoryMapper;

    public CategoryService(
        ICategoryRepository categoryRepository,
        IMapper<Category, CategoryDto> categoryMapper)
    {
        _categoryRepository = categoryRepository;
        _categoryMapper = categoryMapper;
    }

    public async Task<ICollection<CategoryDto>> GetAll()
    {
        var categories = await _categoryRepository.GetAll();
        return categories.Select(category => _categoryMapper.Map(category)).ToList();
    }
}