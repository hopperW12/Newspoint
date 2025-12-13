using FluentValidation;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IValidator<User> _userValidator;

    public UserService(IUserRepository userRepository, IValidator<User> userValidator)
    {
        _userRepository = userRepository;
        _userValidator = userValidator;
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
        // Validace vstupních dat.
        var validationResult = await _userValidator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<User>.Error(ResultErrorType.Validation, firstError);
        }

        // Kontrola, že e-mail už není zaregistrovaný.
        var existingUser = await _userRepository.GetByEmail(entity.Email);
        if (existingUser != null)
            return Result<User>.Error(ResultErrorType.Validation, ServiceMessages.UserEmailExist);

        // Hashování hesla, nastavení data registrace a výchozí role.
        entity.Password = PasswordHasher.HashPassword(entity.Password);
        entity.RegisteredAt = DateTime.Now;
        entity.Role = Role.Reader;

        // Uložení nového uživatele.
        var result = await _userRepository.Add(entity);
        if (result == null)
            return Result<User>.Error(ResultErrorType.UnknownError, ServiceMessages.UserRegisterError);

        return Result<User>.Ok(result);
    }

    public async Task<Result<User>> Update(User entity)
    {
        // Validace vstupních dat.
        var validationResult = await _userValidator.ValidateAsync(entity);
        if (!validationResult.IsValid)
        {
            var firstError = validationResult.Errors.FirstOrDefault()?.ErrorMessage ?? ServiceMessages.Error;
            return Result<User>.Error(ResultErrorType.Validation, firstError);
        }

        // Načtení uživatele z databáze podle e-mailu.
        var existingUser = await _userRepository.GetByEmail(entity.Email);
        if (existingUser == null)
            return Result<User>.Error(ResultErrorType.NotFound, ServiceMessages.UserNotFound);

        // Aktualizace základních údajů a role.
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
        // Pokus o smazání uživatele – repository vrací bool podle úspěchu.
        var deleted = await _userRepository.Delete(id);
        if (!deleted)
            return Result.Error(ResultErrorType.NotFound, ServiceMessages.UserNotFound);

        return Result.Ok();
    }
}
