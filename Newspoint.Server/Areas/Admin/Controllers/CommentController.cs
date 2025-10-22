using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Admin.DTOs;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Admin.Controllers;

[ApiController]
[Area("admin")]
[Route("api/[area]/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentController(ICommentService commentService, IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentCreateDto commentDto)
    {
        var comment = _mapper.Map<Comment>(commentDto);
        var result = await _commentService.Add(comment);
        return this.ToActionResult<Comment, CommentDto>(result, _mapper);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] CommentUpdateDto commentDto)
    {
        var comment = _mapper.Map<Comment>(commentDto);
        var result = await _commentService.Update(comment);
        return this.ToActionResult<Comment, CommentDto>(result, _mapper);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.Delete(id);
        return this.ToActionResult(result);
    }
}