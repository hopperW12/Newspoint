using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface IArticleRepository : IRepository
{
    Task<ICollection<Article>> GetAll();
    Task<Article?> GetById(int id);
    Task<Article?> Add(Article entity);
    Task<Article?> Update(Article entity);
    Task<bool> Delete(int id);
}

public class ArticleRepository : IArticleRepository
{
    private readonly DataDbContext _dataDbContext;

    public ArticleRepository(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<ICollection<Article>> GetAll()
    {
        var articles = _dataDbContext.Articles
            .Include(e => e.Author)
            .Include(e => e.Category)
            .ToListAsync();
        return await articles;
    }

    public Task<Article?> GetById(int id)
    {
        return _dataDbContext.Articles
            .Include(e => e.Author)
            .Include(e => e.Category)
            .Include(e => e.Comments)
                .ThenInclude(e => e.Author)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Article?> Add(Article entity)
    {
        await _dataDbContext.Articles.AddAsync(entity);
        await _dataDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<Article?> Update(Article entity)
    {
        _dataDbContext.Articles.Update(entity);
        await _dataDbContext.SaveChangesAsync();

        return entity;
    }

    public async Task<bool> Delete(int id)
    {
        var entity = await _dataDbContext.Articles
            .FirstOrDefaultAsync(e => e.Id == id);
        if (entity == null)
            return false;

        _dataDbContext.Articles.Remove(entity);
        await _dataDbContext.SaveChangesAsync();

        return true;
    }
}