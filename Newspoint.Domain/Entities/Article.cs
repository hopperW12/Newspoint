namespace Newspoint.Domain.Entities;

public class Article : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public DateTime PublishedAt { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    public int AuthorId { get; set; }
    public User Author { get; set; }
    
    public ICollection<Comment> Comments { get; set; }
}