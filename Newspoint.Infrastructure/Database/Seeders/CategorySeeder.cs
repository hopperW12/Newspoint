using Newspoint.Domain.Entities;

namespace Newspoint.Infrastructure.Database.Seeders;

public class CategorySeeder : IEntitySeeder<Category>
{
    public ICollection<Category> GetEntities() =>
        new List<Category>
        {
            new ()
            {
                Id = 1,
                Name = "Sport"
            },
            new ()
            {
                Id = 2,
                Name = "Politika"
            },
            new ()
            {
                Id = 3,
                Name = "Technologie"
            }
        };
}