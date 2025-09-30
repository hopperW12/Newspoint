namespace Newspoint.Domain.Entities;

public class Category : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    
    public ICollection<Article> Articles { get; set; }
}