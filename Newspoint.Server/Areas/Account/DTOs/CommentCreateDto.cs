namespace Newspoint.Server.Areas.Account.DTOs;

public class CommentCreateDto
{
    public string Content { get; set; }
    public int ArticleId { get; set; }
}