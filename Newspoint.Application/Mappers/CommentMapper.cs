using Newspoint.Application.DTOs;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Mappers;

public class CommentMapper : IMapper<Comment, CommentDto>
{
    public CommentDto Map(Comment entity)
    {
        return new CommentDto
        {
            Id = entity.Id,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            ArticleId = entity.ArticleId,
            Article = entity.Article.Title,
            AuthorId = entity.AuthorId,
            Author = $"{entity.Author.FirstName} {entity.Author.LastName}"
        };
    }

    public Comment MapBack(CommentDto dto)
    {
        var names = dto.Author.Split(' ', 2);

        return new Comment
        {
            Id = dto.Id,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            ArticleId = dto.ArticleId,
            Article = new Article
            {
                Id = dto.ArticleId,
                Title = dto.Article
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