using Newspoint.Domain.Entities;

namespace Newspoint.Application.Services.Interfaces;

public interface IUserService : IService
{
    Task<ICollection<User>> GetAll();
    Task<User?> GetByEmail(string email);
    Task<Result<User>> Add(User entity);
    Task<Result<User>> Update(User entity);
    Task<Result> Delete(int id);
}
