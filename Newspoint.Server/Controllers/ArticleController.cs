using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;

namespace Newspoint.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpGet]
    public async Task<IEnumerable<Article>> GetAll()
    {
        return await _articleService.GetAll();
    }
}