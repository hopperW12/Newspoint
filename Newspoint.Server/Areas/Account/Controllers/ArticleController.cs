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
[Route("api/[area]/[controller]")]
public class ArticleController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IArticleService _articleService;
    private readonly IArticleImageService _articleImageService;
    private readonly IMapper _mapper;

    public ArticleController(
        IUserService userService,
        IArticleService articleService,
        IArticleImageService articleImageService,
        IMapper mapper)
    {
        _userService = userService;
        _articleService = articleService;
        _articleImageService = articleImageService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetArticles()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var articles = await _articleService.GetUserArticles(user.Id);
        return Ok(_mapper.Map<IEnumerable<ArticleDto>>(articles));
    }
    
    [HttpPost]
    public async Task<IActionResult> AddArticle([FromForm] AccountArticleCreateDto accountArticleDto, IFormFile? image)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();

        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();

        var article = _mapper.Map<Article>(accountArticleDto);
        article.AuthorId = user.Id;

        if (image != null && image.Length > 0)
        {
            try
            {
                await using var stream = image.OpenReadStream();
                article.ImagePath = await _articleImageService.SaveImage(
                    image.FileName,
                    image.ContentType,
                    stream);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        var result = await _articleService.Add(article);
        if (!result.Success)
            await _articleImageService.DeleteImage(article.ImagePath);
        
        return this.ToActionResult<Article, ArticleDto>(result, _mapper);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var canDelete = await _articleService.CanUserDelete(user.Id, id);
        if (!canDelete.Success)
            return this.ToActionResult(canDelete);

        var existingResult = await _articleService.GetById(id);
        if (!existingResult.Success || existingResult.Data == null)
            return this.ToActionResult(existingResult);

        var existingArticle = existingResult.Data;

        var result = await _articleService.Delete(id);
        if (!result.Success)
            return this.ToActionResult(result);

        await _articleImageService.DeleteImage(existingArticle.ImagePath);

        return this.ToActionResult(result);
    }
}