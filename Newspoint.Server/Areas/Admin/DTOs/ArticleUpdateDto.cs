namespace Newspoint.Server.Areas.Admin.DTOs;

public class ArticleUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }

    public string? ImagePath { get; set; }
}
