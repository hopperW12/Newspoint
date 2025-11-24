using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Account.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Account.Controllers;

[Authorize]
[ApiController]
[Area("account")]
[Route("api/[area]")]
public class ArticleController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IArticleService _articleService;
    private readonly IMapper _mapper;

    public ArticleController(
        IUserService userService,
        IArticleService articleService,
        IMapper mapper)
    {
        _userService = userService;
        _articleService = articleService;
        _mapper = mapper;
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var articles = await _articleService.GetUserArticles(user.Id);
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(articles));
    }
    
    [HttpPost("article")]
    public async Task<IActionResult> AddArticle([FromBody] AccountArticleCreateDto accountArticleDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var article = _mapper.Map<Article>(accountArticleDto);
        article.AuthorId = user.Id;
        
        var result = await _articleService.Add(article);
        return this.ToActionResult<Article, ArticleDto>(result, _mapper);
    }
    
    [HttpDelete("article/{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var canDelete = await _articleService.CanUserDelete(user.Id, id);
        if (!canDelete.Success)
            return this.ToActionResult(canDelete);
        
        var result = await _articleService.Delete(id);
        return this.ToActionResult(result);
    }
}