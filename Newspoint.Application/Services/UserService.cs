using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories;

namespace Newspoint.Application.Services;

public interface IUserService : IService
{
    Task<User?> GetByEmail(string email);
    Task<Result> Register(User entity);
    Task<bool> Delete(int id);
}

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User?> GetByEmail(string email)
    {
        return _userRepository.GetByEmail(email);
    }

    public async Task<Result> Register(User entity)
    {
        var existingUser = await _userRepository.GetByEmail(entity.Email);
        if (existingUser != null)
            return Result.Error(ResultErrorType.UnknownError, ServiceMessages.UserEmailExist);
        
        entity.RegisteredAt = DateTime.Now;
        entity.Role = Role.Reader;
        
        var result = await _userRepository.Add(entity);
        if (result == null)
            return Result.Error(ResultErrorType.UnknownError, ServiceMessages.UserRegisterError);
        
        return Result.Ok();
    }

    public Task<bool> Delete(int id)
    {
        return _userRepository.Delete(id);   
    }
}