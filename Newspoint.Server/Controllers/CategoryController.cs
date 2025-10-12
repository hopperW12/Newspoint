using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.DTOs;
using Newspoint.Application.Services;

namespace Newspoint.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IEnumerable<CategoryDto>> GetAll()
        {
            return await _categoryService.GetAll();
        }
    }
}