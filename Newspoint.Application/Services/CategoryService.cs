using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface ICategoryService : IService
{
    Task<ICollection<Category>> GetAll();
}

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<ICollection<Category>> GetAll()
    {
        return _categoryRepository.GetAll();
    }
}