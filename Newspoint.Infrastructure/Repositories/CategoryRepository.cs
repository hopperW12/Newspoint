using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;
using Newspoint.Infrastructure.Database;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Infrastructure.Repositories;

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