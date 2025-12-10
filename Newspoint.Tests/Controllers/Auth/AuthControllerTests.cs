using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Auth.Controllers;
using Newspoint.Server.Areas.Auth.DTOs;

namespace Newspoint.Tests.Controllers.Auth;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c[It.IsAny<string>()]).Returns<string?>(null);

        _controller = new AuthController(
            _mockService.Object,
            _mockMapper.Object,
            _mockConfiguration.Object);
    }

    // Login

    [Fact]
    public async Task Login_When_CredentialsValid_ReturnsOkWithToken()
    {
        // Arrange
        var password = "secret";
        var user = new User
        {
            Id = 1,
            Email = "a@a.com",
            Password = PasswordHasher.HashPassword(password),
            FirstName = "A",
            LastName = "B",
            Role = Role.Reader
        };

        _mockService.Setup(s => s.GetByEmail(user.Email))
            .ReturnsAsync(user);

        var loginDto = new UserLoginDto
        {
            Email = user.Email,
            Password = password
        };

        // Test
        var actionResult = await _controller.Login(loginDto);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = okResult.Value;

        Assert.NotNull(value);
        var prop = value.GetType().GetProperty("Token", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var token = prop!.GetValue(value) as string;
        Assert.False(string.IsNullOrEmpty(token));

        _mockService.Verify(s => s.GetByEmail(user.Email), Times.Once);
    }

    [Fact]
    public async Task Login_When_UserNotFound_ReturnsUnauthorized()
    {
        // Arrange
        _mockService.Setup(s => s.GetByEmail(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        var loginDto = new UserLoginDto
        {
            Email = "no@one.com",
            Password = "x"
        };

        // Test
        var actionResult = await _controller.Login(loginDto);

        Assert.IsType<UnauthorizedResult>(actionResult);
        _mockService.Verify(s => s.GetByEmail(loginDto.Email), Times.Once);
    }

    // Register

    [Fact]
    public async Task Register_When_ServiceReturnsOk_ReturnsOkWithToken()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "new@u.com",
            Password = "p",
            FirstName = "F",
            LastName = "L"
        };

        var user = new User
        {
            Id = 5,
            Email = registerDto.Email!,
            FirstName = registerDto.FirstName!,
            LastName = registerDto.LastName!,
            Role = Role.Reader
        };

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserRegisterDto>()))
            .Returns(user);

        _mockService.Setup(s => s.Add(It.IsAny<User>()))
            .ReturnsAsync(Result<User>.Ok(user));

        // Test
        var actionResult = await _controller.Register(registerDto);
        var okResult = Assert.IsType<OkObjectResult>(actionResult);
        var value = okResult.Value;

        Assert.NotNull(value);
        var prop = value.GetType().GetProperty("Token", BindingFlags.Public | BindingFlags.Instance);
        Assert.NotNull(prop);
        var token = prop!.GetValue(value) as string;
        Assert.False(string.IsNullOrEmpty(token));

        _mockService.Verify(s => s.Add(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Register_When_ServiceReturnsNotFound_ReturnsNotFound()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            Email = "e@e.com"
        };

        var user = new User
        {
            Email = registerDto.Email!
        };

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserRegisterDto>()))
            .Returns(user);

        _mockService.Setup(s => s.Add(It.IsAny<User>()))
            .ReturnsAsync(Result<User>.Error(ResultErrorType.NotFound, "err"));

        // Test
        var actionResult = await _controller.Register(registerDto);

        Assert.IsType<NotFoundObjectResult>(actionResult);
        _mockService.Verify(s => s.Add(It.IsAny<User>()), Times.Once);
    }
}
