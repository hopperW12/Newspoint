namespace Newspoint.Domain.Entities;

public class User : IEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Role Role { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    public DateTime RegisteredAt { get; set; }
}
