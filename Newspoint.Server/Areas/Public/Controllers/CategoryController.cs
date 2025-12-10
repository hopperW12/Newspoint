using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Server.Areas.Public.DTOs;

namespace Newspoint.Server.Areas.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryController(ICategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IEnumerable<CategoryDto>> GetAll()
    {
        var categories = await _categoryService.GetAll();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }
}
