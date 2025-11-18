using Newspoint.Domain.Entities;
using Newspoint.Domain.Interfaces;

namespace Newspoint.Infrastructure.Repositories.Interfaces;

public interface IUserRepository : IRepository
{
    public Task<User?> GetById(int id);
    Task<User?> GetByEmail(string email);
    Task<User?> Add(User user);
    Task<bool> Delete(int id);
}