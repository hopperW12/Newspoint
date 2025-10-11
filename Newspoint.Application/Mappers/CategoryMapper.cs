using Newspoint.Application.DTOs;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Mappers;

public class CategoryMapper : IMapper<Category, CategoryDto>
{
    public CategoryDto Map(Category entity)
    {
        return new CategoryDto
        {
            Name = entity.Name
        };
    }

    public Category MapBack(CategoryDto dto)
    {
        return new Category
        {
            Name = dto.Name
        };
    }
}