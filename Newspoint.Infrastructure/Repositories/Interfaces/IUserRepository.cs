using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;

namespace Newspoint.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository
{
    Task<ICollection<User>> GetAll();
    Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User?> Add(User entity);
    Task<User?> Update(User entity);
    Task<bool> Delete(int id);
}
