using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Admin.Controllers;

[ApiController]
[Area("admin")]
[Route("api/[area]/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IMapper<Article, ArticleDto> _articleMapper;
    private readonly IMapper<Article, ArticleCreateDto> _articleCreateMapper;
    private readonly IMapper<Article, ArticleUpdateDto> _articleUpdateMapper;

    public ArticleController(
        IArticleService articleService,
        IMapper<Article, ArticleDto> articleMapper,
        IMapper<Article, ArticleCreateDto> articleCreateMapper,
        IMapper<Article, ArticleUpdateDto> articleUpdateMapper)
    {
        _articleService = articleService;
        _articleMapper = articleMapper;
        _articleCreateMapper = articleCreateMapper;
        _articleUpdateMapper = articleUpdateMapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddArticle([FromBody] ArticleCreateDto articleDto)
    {
        var article = _articleCreateMapper.MapBack(articleDto);
        var result = await _articleService.Add(article);
        return this.ToActionResult(result, _articleMapper);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromBody] ArticleUpdateDto articleDto)
    {
        var article = _articleUpdateMapper.MapBack(articleDto);
        var result = await _articleService.Update(article);
        return this.ToActionResult(result, _articleMapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var result = await _articleService.Delete(id);
        return this.ToActionResult(result);
    }
}