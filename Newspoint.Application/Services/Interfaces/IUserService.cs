using Newspoint.Domain.Entities;

namespace Newspoint.Application.Services.Interfaces;

public interface IUserService : IService
{
    Task<User?> GetByEmail(string email);
    Task<Result> Register(User entity);
    Task<bool> Delete(int id);
}