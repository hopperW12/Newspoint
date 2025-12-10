namespace Newspoint.Server.Areas.Account.DTOs;

public class AccountArticleCreateDto
{
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public int CategoryId { get; set; }

    public string? ImagePath { get; set; }
}
