using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.DTOs;
using Newspoint.Application.Services;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IEnumerable<ArticleDto>> GetArticles()
    {
        return await _articleService.GetAll();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _articleService.GetByIdWithComments(id);
        return this.ToActionResult(result);

    }

    [HttpPost]
    public async Task<IActionResult> AddArticle([FromBody] ArticleDto article)
    {
        var result = await _articleService.Add(article);
        return this.ToActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromBody] ArticleDto article)
    {
        var result = await _articleService.Update(article);
        return this.ToActionResult(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var result = await _articleService.Delete(id);
        return this.ToActionResult(result);
    }
}