using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.Controllers;
using Newspoint.Server.Areas.Admin.DTOs;

namespace Newspoint.Tests.Controllers.Admin;

public class UserControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserController _controller;

    public UserControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new UserController(_mockService.Object, _mockMapper.Object);
    }

    // Get All

    [Fact]
    public async Task GetAll_When_UsersExist_ReturnsOk()
    {
        // Arrange
        var users = new List<User>();
        var mapped = new List<UserDto>();

        _mockService.Setup(s => s.GetAll())
            .ReturnsAsync(users);

        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
            .Returns(mapped);

        // Test
        var action = await _controller.GetAll();
        var ok = Assert.IsType<OkObjectResult>(action);

        Assert.Equal(mapped, ok.Value);
        _mockService.Verify(s => s.GetAll(), Times.Once);
        _mockMapper.Verify(m => m.Map<IEnumerable<UserDto>>(users), Times.Once);
    }

    // Add User

    [Fact]
    public async Task AddUser_When_ServiceOk_ReturnsOk()
    {
        // Arrange
        var dto = new UserCreateDto { Email = "a@a.com" };
        var user = new User { Id = 7, Email = dto.Email! };
        var userDto = new UserDto { Id = 7, Email = dto.Email! };

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserCreateDto>()))
            .Returns(user);

        _mockService.Setup(s => s.Add(It.IsAny<User>()))
            .ReturnsAsync(Result<User>.Ok(user));

        _mockMapper.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(userDto);

        // Test
        var action = await _controller.AddUser(dto);
        var ok = Assert.IsType<OkObjectResult>(action);
        var result = Assert.IsType<Result<UserDto>>(ok.Value);

        Assert.True(result.Success);
        Assert.Equal(userDto, result.Data);
        _mockService.Verify(s => s.Add(It.IsAny<User>()), Times.Once);
    }

    // Update User

    [Fact]
    public async Task UpdateUser_When_ServiceReturnsNotFound_ReturnsNotFound()
    {
        // Arrange
        var dto = new UserUpdateDto { Id = 1 };
        var user = new User { Id = 1 };

        _mockMapper.Setup(m => m.Map<User>(It.IsAny<UserUpdateDto>()))
            .Returns(user);

        _mockService.Setup(s => s.Update(It.IsAny<User>()))
            .ReturnsAsync(Result<User>.Error(ResultErrorType.NotFound, "x"));

        // Test
        var action = await _controller.UpdateUser(dto);

        Assert.IsType<NotFoundObjectResult>(action);
        _mockService.Verify(s => s.Update(It.IsAny<User>()), Times.Once);
    }
}
