using Microsoft.EntityFrameworkCore;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Database;

namespace Newspoint.Application.Services;

public interface IArticleService : IService
{
    Task<ICollection<Article>> GetAll();
}
public class ArticleService : IArticleService
{
    private readonly DataDbContext _dataDbContext;
    
    public ArticleService(DataDbContext dataDbContext)
    {
        _dataDbContext = dataDbContext;
    }

    public async Task<ICollection<Article>> GetAll()
    {
        var articles = _dataDbContext.Articles.ToListAsync();
        return await articles;
    }
}