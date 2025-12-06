namespace Newspoint.Server.Areas.Account.DTOs;

public class AccountArticleUpdateDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int CategoryId { get; set; }
    public int AuthorId { get; set; }

    public string? ImagePath { get; set; }
}
