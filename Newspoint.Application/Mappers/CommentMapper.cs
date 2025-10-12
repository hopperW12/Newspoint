using Newspoint.Application.DTOs;
using Newspoint.Domain.Entities;

namespace Newspoint.Application.Mappers;

public class CommentMapper : IMapper<Comment, CommentDto>
{
    public CommentDto Map(Comment entity)
    {
        return new CommentDto
        {
            Content = entity.Content,
            PublishedAt = entity.PublishedAt,
            Author = $"{entity.Author.FirstName} {entity.Author.LastName}"
        };
    }

    public Comment MapBack(CommentDto dto)
    {
        var names = dto.Author.Split(' ', 2);

        return new Comment
        {
            Content = dto.Content,
            PublishedAt = dto.PublishedAt,
            Author = new User
            {
                FirstName = names[0],
                LastName = names[1]
            }
        };
    }
}