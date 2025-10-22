using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Admin.DTOs;

public class CommentCreateDto : IEntityDto
{
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int AuthorId { get; set; }
    public int ArticleId { get; set; }
}