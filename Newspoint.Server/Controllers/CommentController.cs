using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.DTOs;
using Newspoint.Application.Services;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    
    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var result = await _commentService.GetById(id);
        return this.ToActionResult(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] CommentDto comment)
    {
        var result = await _commentService.Add(comment);
        return this.ToActionResult(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateComment([FromBody] CommentDto comment)
    {
        var result = await _commentService.Update(comment);
        return this.ToActionResult(result);
    }
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var result = await _commentService.Delete(id);
        return this.ToActionResult(result);
    }
}