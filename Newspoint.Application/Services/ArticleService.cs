using Newspoint.Application.DTOs;
using Newspoint.Application.Mappers;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface IArticleService : IService
{
    Task<ICollection<ArticleDto>> GetAll();
}
public class ArticleService : IArticleService
{
    private readonly IArticleRepository _articleRepository;
    private readonly IMapper<Article, ArticleDto> _articleMapper;

    public ArticleService(
        IArticleRepository articleRepository,
        IMapper<Article, ArticleDto> articleMapper)
    {
        _articleRepository = articleRepository;
        _articleMapper = articleMapper;
    }

    public async Task<ICollection<ArticleDto>> GetAll()
    {
        var articles = await _articleRepository.GetAll();
        return articles.Select(article => _articleMapper.Map(article)).ToList();
    }
}