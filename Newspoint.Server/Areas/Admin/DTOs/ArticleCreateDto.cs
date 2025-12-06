namespace Newspoint.Server.Areas.Admin.DTOs;

public class ArticleCreateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }

    public string? ImagePath { get; set; }
}