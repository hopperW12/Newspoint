using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Account.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Account.Controllers;

[Authorize]
[ApiController]
[Area("account")]
[Route("api/[area]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public AccountController(IUserService userService, ICommentService commentService, IMapper mapper)
    {
        _userService = userService;
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet("comments")]
    public async Task<IActionResult> GetComments()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var comments = await _commentService.GetUserComments(user.Id);
        return Ok(_mapper.Map<IEnumerable<CommentDto>>(comments));
    }

    [HttpPost("comment")]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDto commentDto)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var comment = _mapper.Map<Comment>(commentDto);
        comment.AuthorId = user.Id;
        
        var result = await _commentService.Add(comment);
        return this.ToActionResult<Comment, CommentDto>(result, _mapper);
    }

    [HttpDelete("comment/{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email == null) return Unauthorized();
        
        var user = await _userService.GetByEmail(email);
        if (user == null) return Unauthorized();
        
        var canDelete = await _commentService.CanUserDelete(user.Id, id);
        if (!canDelete.Success)
            return this.ToActionResult(canDelete);
        
        var result = await _commentService.Delete(id);
        return this.ToActionResult(result);
    }
}