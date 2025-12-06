using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Admin.Controllers;

[ApiController]
[Area("admin")]
[Route("api/[area]/[controller]")]
[Authorize(Roles = $"{nameof(Role.Admin)},{nameof(Role.Editor)}")]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;
    private readonly IArticleImageService _articleImageService;
    private readonly IMapper _mapper;

    public ArticleController(IArticleService articleService, IArticleImageService articleImageService, IMapper mapper)
    {
        _articleService = articleService;
        _articleImageService = articleImageService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddArticle([FromForm] ArticleCreateDto articleDto, IFormFile? image)
    {
        var article = _mapper.Map<Article>(articleDto);

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

    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromForm] ArticleUpdateDto articleDto, IFormFile? image, [FromForm] bool deleteImage = false)
    {
        var existingResult = await _articleService.GetById(articleDto.Id);
        if (!existingResult.Success || existingResult.Data == null)
            return this.ToActionResult(existingResult);

        var existingArticle = existingResult.Data;
        var article = _mapper.Map<Article>(articleDto);
        article.ImagePath = existingArticle.ImagePath;

        if (deleteImage)
        {
            await _articleImageService.DeleteImage(existingArticle.ImagePath);
            article.ImagePath = null;
        }

        if (image != null && image.Length > 0)
        {
            try
            {
                await using var stream = image.OpenReadStream();
                article.ImagePath = await _articleImageService.ReplaceImage(
                    existingArticle.ImagePath,
                    image.FileName,
                    image.ContentType,
                    stream);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        var result = await _articleService.Update(article);
        return this.ToActionResult<Article, ArticleDto>(result, _mapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
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