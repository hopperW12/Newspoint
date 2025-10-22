using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IMapper<Article, ArticleDto> _mapper;

    public ArticleController(
        IArticleService articleService,
        IMapper<Article, ArticleDto> mapper)
    {
        _articleService = articleService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<ArticleDto>> GetArticles()
    {
        var articles = await _articleService.GetAll();
        return articles.Select(a => _mapper.Map(a)).ToList();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _articleService.GetByIdWithComments(id);
        return this.ToActionResult(result, _mapper);
    }
}