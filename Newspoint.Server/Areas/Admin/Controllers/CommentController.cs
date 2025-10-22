using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Admin.Controllers;

[ApiController]
[Area("admin")]
[Route("api/[area]/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper<Comment, CommentDto> _commentMapper;
    private readonly IMapper<Comment, CommentCreateDto> _commentCreateMapper;
    private readonly IMapper<Comment, CommentUpdateDto> _commentUpdateMapper;

    public CommentController(
        ICommentService commentService,
        IMapper<Comment, CommentDto> commentMapper,
        IMapper<Comment, CommentCreateDto> commentCreateMapper,
        IMapper<Comment, CommentUpdateDto> commentUpdateMapper)
    {
        _commentService = commentService;
        _commentMapper = commentMapper;
        _commentCreateMapper = commentCreateMapper;
        _commentUpdateMapper = commentUpdateMapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDto commentDto)
    {
        var comment = _commentCreateMapper.MapBack(commentDto);
        var result = await _commentService.Add(comment);
        return this.ToActionResult(result, _commentMapper);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDto commentDto)
    {
        var comment = _commentUpdateMapper.MapBack(commentDto);
        var result = await _commentService.Update(comment);
        return this.ToActionResult(result, _commentMapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.Delete(id);
        return this.ToActionResult(result);
    }
}