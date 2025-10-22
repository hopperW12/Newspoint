using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.Mappers;

public class CategoryMapper : IMapper<Category, CategoryDto>
{
    public CategoryDto Map(Category entity)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    public Category MapBack(CategoryDto dto)
    {
        return new Category
        {
            Id = dto.Id,
            Name = dto.Name
        };
    }
}
