using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Admin.Mappers;

public class CommentCreateMapper : IMapper<Comment, CommentCreateDto>
{
    public CommentCreateDto Map(Comment entity)
    {
        return new CommentCreateDto
        {
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            ArticleId = entity.ArticleId,
            AuthorId = entity.AuthorId
        };
    }

    public Comment MapBack(CommentCreateDto dto)
    {
        return new Comment
        {
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            ArticleId = dto.ArticleId,
            AuthorId = dto.AuthorId
        };
    }
}

public class CommentUpdateMapper : IMapper<Comment, CommentUpdateDto>
{
    public CommentUpdateDto Map(Comment entity)
    {
        return new CommentUpdateDto
        {
            Id = entity.Id,
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            ArticleId = entity.ArticleId,
            AuthorId = entity.AuthorId
        };
    }

    public Comment MapBack(CommentUpdateDto dto)
    {
        return new Comment
        {
            Id = dto.Id,
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            ArticleId = dto.ArticleId,
            AuthorId = dto.AuthorId
        };
    }
}