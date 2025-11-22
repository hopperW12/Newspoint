using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;

namespace Newspoint.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository : IRepository
{
    Task<ICollection<Category>> GetAll();
    Task<Category?> GetById(int id);
}
