using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;

namespace Newspoint.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IEnumerable<Category>> GetAll()
        {
            return await _categoryService.GetAll();
        }
    }
}