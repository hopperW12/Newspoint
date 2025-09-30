namespace Newspoint.Domain.Entities;

public class Comment : IEntity
{
    public int Id { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    
    public int AuthorId { get; set; }
    public User Author { get; set; }
    
    public int ArticleId { get; set; }
    public Article Article { get; set; }
}