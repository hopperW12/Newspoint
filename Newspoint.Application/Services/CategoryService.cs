using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<ICollection<Category>> GetAll()
    {
        return _categoryRepository.GetAll();
    }
}