using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Application.Services;

public interface ICategoryService : IService
{
    Task<ICollection<Category>> GetAll();
}

public class CategoryService : ICategoryService
{
    private readonly DataDbContext _dataDbContext;
    
    public CategoryService(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }
    
    public async Task<ICollection<Category>> GetAll()
    {
        var categories = _dataDbContext.Categories.ToListAsync();
        return await categories;
    }
}