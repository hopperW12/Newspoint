using FluentValidation;
using FluentValidation.Results;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Infrastructure.Repositories.Interfaces;

namespace Newspoint.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IValidator<User>> _mockValidator;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockValidator = new Mock<IValidator<User>>();
        _service = new UserService(_mockRepository.Object, _mockValidator.Object);
    }

    private static ValidationResult ValidResult() => new ();

    private static ValidationResult InvalidResult(string error) => new ([new ValidationFailure("prop", error)]);

    // Add

    [Fact]
    public async Task Add_When_ValidAndNotExists_ReturnsOkWithHashedPasswordAndReaderRole()
    {
        // Arrange
        var user = new User { Email = "test@test.com", Password = "plain", FirstName = "A", LastName = "B" };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync((User?)null);

        _mockRepository.Setup(r => r.Add(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);

        // Test
        var result = await _service.Add(user);

        Assert.True(result.Success);
        Assert.NotNull(result.Data);
        Assert.Equal(Role.Reader, result.Data!.Role);
        Assert.NotEqual("plain", result.Data.Password);
        Assert.NotEqual(default, result.Data.RegisteredAt);

        _mockValidator.Verify(v => v.ValidateAsync(user, CancellationToken.None), Times.Once);
        _mockRepository.Verify(r => r.GetByEmail(user.Email), Times.Once);
        _mockRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Add_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var user = new User { Email = "bad@test.com", Password = "p" };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Add(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
        _mockRepository.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
        _mockRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Add_When_EmailAlreadyExists_ReturnsValidationError()
    {
        // Arrange
        var user = new User { Email = "dup@test.com", Password = "p" };
        var existing = new User { Email = user.Email };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync(existing);

        // Test
        var result = await _service.Add(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
        _mockRepository.Verify(r => r.GetByEmail(user.Email), Times.Once);
        _mockRepository.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Add_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var user = new User { Email = "t@test.com", Password = "p" };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync((User?)null);

        _mockRepository.Setup(r => r.Add(It.IsAny<User>()))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.Add(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Update

    [Fact]
    public async Task Update_When_ValidAndUserExists_ReturnsOk()
    {
        // Arrange
        var user = new User { Email = "t@test.com", FirstName = "A", LastName = "B", Role = Role.Editor };
        var existing = new User { Email = user.Email, FirstName = "Old", LastName = "User", Role = Role.Reader };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync(existing);

        _mockRepository.Setup(r => r.Update(existing))
            .ReturnsAsync(existing);

        // Test
        var result = await _service.Update(user);

        Assert.True(result.Success);
        Assert.Equal(user.FirstName, existing.FirstName);
        Assert.Equal(user.LastName, existing.LastName);
        Assert.Equal(user.Role, existing.Role);
        _mockRepository.Verify(r => r.Update(existing), Times.Once);
    }

    [Fact]
    public async Task Update_When_ValidationFails_ReturnsValidationError()
    {
        // Arrange
        var user = new User { Email = "bad@test.com" };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(InvalidResult("err"));

        // Test
        var result = await _service.Update(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.Validation, result.ErrorType);
        _mockRepository.Verify(r => r.GetByEmail(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Update_When_UserNotFound_ReturnsNotFound()
    {
        // Arrange
        var user = new User { Email = "none@test.com" };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.Update(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task Update_When_RepositoryReturnsNull_ReturnsUnknownError()
    {
        // Arrange
        var user = new User { Email = "t@test.com" };
        var existing = new User { Email = user.Email };

        _mockValidator.Setup(v => v.ValidateAsync(user, CancellationToken.None))
            .ReturnsAsync(ValidResult());

        _mockRepository.Setup(r => r.GetByEmail(user.Email))
            .ReturnsAsync(existing);

        _mockRepository.Setup(r => r.Update(existing))
            .ReturnsAsync((User?)null);

        // Test
        var result = await _service.Update(user);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.UnknownError, result.ErrorType);
    }

    // Delete

    [Fact]
    public async Task Delete_When_RepositoryReturnsTrue_ReturnsOk()
    {
        // Arrange
        _mockRepository.Setup(r => r.Delete(1))
            .ReturnsAsync(true);

        // Test
        var result = await _service.Delete(1);

        Assert.True(result.Success);
        _mockRepository.Verify(r => r.Delete(1), Times.Once);
    }

    [Fact]
    public async Task Delete_When_RepositoryReturnsFalse_ReturnsNotFound()
    {
        // Arrange
        _mockRepository.Setup(r => r.Delete(2))
            .ReturnsAsync(false);

        // Test
        var result = await _service.Delete(2);

        Assert.False(result.Success);
        Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
    }
}
