using Newspoint.Domain.Entities;

namespace Newspoint.Infrastructure.Database.Seeders;

public interface IEntitySeeder<TEntity> where TEntity : IEntity
{
    ICollection<TEntity> GetEntities();
}
