using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.Mappers;

public class ArticleMapper : IMapper<Article, ArticleDto>
{
    private readonly IMapper<Comment, CommentDto> _commentMapper;
    
    public ArticleMapper(IMapper<Comment, CommentDto> commentMapper)
    {
        _commentMapper = commentMapper;
    }
    
    public ArticleDto Map(Article entity)
    {
        return new ArticleDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            CategoryId = entity.CategoryId,
            Category = entity.Category.Name,
            AuthorId = entity.AuthorId,
            Author = $"{entity.Author.FirstName} {entity.Author.LastName}",
            Comments = entity.Comments.Select(_commentMapper.Map).ToList()
        };
    }

    public Article MapBack(ArticleDto dto)
    {
        var names = dto.Author.Split(' ', 2);

        return new Article
        {
            Id = dto.Id,
            Title = dto.Title,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            CategoryId = dto.CategoryId,
            Category = new Category
            {
                Id = dto.CategoryId,
                Name = dto.Category
            },
            AuthorId = dto.AuthorId,
            Author = new User
            {
                Id = dto.AuthorId,
                FirstName = names[0],
                LastName = names[1]
            }
        };
    }
}
