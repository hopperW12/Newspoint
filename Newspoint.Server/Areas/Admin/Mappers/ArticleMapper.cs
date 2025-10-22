using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Admin.Mappers;

public class ArticleCreateMapper : IMapper<Article, ArticleCreateDto>
{
    public ArticleCreateDto Map(Article entity)
    {
        return new ArticleCreateDto
        {
            Title = entity.Title,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            AuthorId = entity.AuthorId,
            CategoryId = entity.CategoryId
        };
    }

    public Article MapBack(ArticleCreateDto dto)
    {
        return new Article
        {
            Title = dto.Title,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            AuthorId = dto.AuthorId,
            CategoryId = dto.CategoryId
        };
    }
}

public class ArticleUpdateMapper : IMapper<Article, ArticleUpdateDto>
{
    public ArticleUpdateDto Map(Article entity)
    {
        return new ArticleUpdateDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            CategoryId = entity.CategoryId,
            AuthorId = entity.AuthorId
        };
    }

    public Article MapBack(ArticleUpdateDto dto)
    {
        return new Article
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            CategoryId = dto.CategoryId,
            AuthorId = dto.AuthorId,
        };
    }
}