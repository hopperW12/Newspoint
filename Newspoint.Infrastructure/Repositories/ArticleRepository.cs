using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Infrastructure.Repositories;

public interface IArticleRepository : IRepository
{
    Task<ICollection<Article>> GetAll();
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
}