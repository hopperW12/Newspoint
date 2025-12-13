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
            Password = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
            Role = Role.Admin
        },
        new ()
        {
            Id = 2,
            FirstName = "redaktor",
            LastName = "redaktor",
            Email = "redaktor@newspoint.com",
            Password = "d437337e6abc848ac36eba2294ca57cb54c052d92ab7ad72071a7ab086761964",
            Role = Role.Editor
        }
    };
}
