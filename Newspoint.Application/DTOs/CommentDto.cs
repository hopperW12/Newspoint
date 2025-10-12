namespace Newspoint.Application.DTOs;

public class CommentDto
{
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Author { get; set; }
}