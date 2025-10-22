using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;
using Newspoint.Server.Interfaces;

namespace Newspoint.Server.Areas.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper<Comment, CommentDto> _mapper;

    public CommentController(
        ICommentService commentService,
        IMapper<Comment, CommentDto> mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var result = await _commentService.GetById(id);
        return this.ToActionResult(result, _mapper);
    }
}