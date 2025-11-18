using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface ICategoryRepository : IRepository
{
    Task<ICollection<Category>> GetAll();
    Task<Category?> GetById(int id);
}

public class CategoryRepository : ICategoryRepository
{
    private readonly DataDbContext _dataDbContext;

    public CategoryRepository(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<ICollection<Category>> GetAll()
    {
        var categories = _dataDbContext.Categories.ToListAsync();
        return await categories;
    }

    public Task<Category?> GetById(int id)
    {
        return _dataDbContext.Categories.FirstOrDefaultAsync(e => e.Id == id);
    }
}