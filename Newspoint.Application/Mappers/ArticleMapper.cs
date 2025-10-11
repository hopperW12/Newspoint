using Newspoint.Application.DTOs;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Mappers;

public class ArticleMapper : IMapper<Article, ArticleDto>
{
    public ArticleDto Map(Article entity)
    {
        return new ArticleDto
        {
            Title = entity.Title,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            Category = entity.Category.Name,
            Author = $"{entity.Author.FirstName} {entity.Author.LastName}"
        };
    }

    public Article MapBack(ArticleDto dto)
    {
        var names = dto.Author.Split(' ', 2);

        return new Article
        {
            Title = dto.Title,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            Category = new Category
            {
                Name = dto.Category
            },
            Author = new User
            {
                FirstName = names.FirstOrDefault() ?? "",
                LastName = names.Skip(1).FirstOrDefault() ?? ""
            }
        };
    }
}