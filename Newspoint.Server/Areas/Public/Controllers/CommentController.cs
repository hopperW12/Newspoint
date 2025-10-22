using Microsoft.AspNetCore.Mvc;
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
}