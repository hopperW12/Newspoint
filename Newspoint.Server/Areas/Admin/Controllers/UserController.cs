using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Admin.Controllers;

[ApiController]
[Area("admin")]
[Route("api/[area]/[controller]")]
[Authorize(Roles = $"{nameof(Role.Admin)}")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public UserController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAll();
        return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserCreateDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        var result = await _userService.Add(user);
        return this.ToActionResult<User, UserDto>(result, _mapper);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userDto)
    {
        var user = _mapper.Map<User>(userDto);
        var result = await _userService.Update(user);
        return this.ToActionResult<User, UserDto>(result, _mapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.Delete(id);
        return this.ToActionResult(result);
    }
}