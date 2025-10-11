using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface IArticleService : IService
{
    Task<ICollection<Article>> GetAll();
}
public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;

    public ArticleService(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public Task<ICollection<Article>> GetAll()
    {
        return _articleRepository.GetAll(); 
    }
}