using AutoMapper;
using Bogus.DataSets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
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
    private readonly IMapper _mapper;

    public ArticleController(IArticleService articleService, IMapper mapper)
    {
        _articleService = articleService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddArticle([FromBody] ArticleCreateDto articleDto)
    {
        var article = _mapper.Map<Article>(articleDto);
        var result = await _articleService.Add(article);
        return this.ToActionResult<Article, ArticleDto>(result, _mapper);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromBody] ArticleUpdateDto articleDto)
    {
        var article = _mapper.Map<Article>(articleDto);
        var result = await _articleService.Update(article);
        return this.ToActionResult<Article, ArticleDto>(result, _mapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var result = await _articleService.Delete(id);
        return this.ToActionResult(result);
    }
}