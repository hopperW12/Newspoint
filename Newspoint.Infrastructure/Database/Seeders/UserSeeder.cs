using Newspoint.Domain.Entities;

namespace Newspoint.Infrastructure.Database.Seeders;

public class UserSeeder : IEntitySeeder<User>
{
    public ICollection<User> GetEntities() => new List<User>
    {
        new()
        {
            Id = 1,
            FirstName = "admin",
            LastName = "admin",
            Email = "admin@newspoint.com",
            Password = "c7ad44cbad762a5da0a452f9e854fdc1e0e7a52a38015f23f3eab1d80b931dd472634dfac71cd34ebc35d16ab7fb8a90c81f975113d6c7538dc69dd8de9077ec",
            Role = Role.Admin
        }
    };
}