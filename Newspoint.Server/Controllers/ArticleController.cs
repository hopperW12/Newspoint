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
}