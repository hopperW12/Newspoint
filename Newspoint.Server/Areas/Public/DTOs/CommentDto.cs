using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.DTOs;

public class CommentDto : IEntityDto
{
    public int Id { get; set; }
    public int ArticleId { get; set; }
    public string Article { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int AuthorId { get; set; }
    public string Author { get; set; }
}