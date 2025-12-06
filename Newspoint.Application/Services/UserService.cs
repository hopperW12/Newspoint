using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<ICollection<User>> GetAll()
    {
        return _userRepository.GetAll();
    }

    public Task<User?> GetByEmail(string email)
    {
        return _userRepository.GetByEmail(email);
    }

    public async Task<Result<User>> Add(User entity)
    {
        var existingUser = await _userRepository.GetByEmail(entity.Email);
        if (existingUser != null)
            return Result<User>.Error(ResultErrorType.UnknownError, ServiceMessages.UserEmailExist);
        
        entity.RegisteredAt = DateTime.Now;
        entity.Role = Role.Reader;
        
        var result = await _userRepository.Add(entity);
        if (result == null)
            return Result<User>.Error(ResultErrorType.UnknownError, ServiceMessages.UserRegisterError);

        return Result<User>.Ok(result);
    }

    public async Task<Result<User>> Update(User entity)
    {
        var existingUser = await _userRepository.GetByEmail(entity.Email);
        if (existingUser == null)
            return Result<User>.Error(ResultErrorType.NotFound, ServiceMessages.UserNotFound);
        
        existingUser.FirstName = entity.FirstName;
        existingUser.LastName = entity.LastName;
        existingUser.Role = entity.Role;
        
        var result = await _userRepository.Update(existingUser);
        if (result == null)
            return Result<User>.Error(ResultErrorType.UnknownError, ServiceMessages.Error);

        return Result<User>.Ok(result);
    }
    

    public async Task<Result> Delete(int id)
    {
        var deleted = await _userRepository.Delete(id);
        if (!deleted)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.UserNotFound);

        return Result.Ok();
    }
}