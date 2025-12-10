namespace Newspoint.Server.Areas.Public.DTOs;

public class ArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }
    public string Category { get; set; }
    public int AuthorId { get; set; }
    public string Author { get; set; }

    public string? ImagePath { get; set; }

    public ICollection<CommentDto> Comments { get; set; } = new List<CommentDto>();
}
