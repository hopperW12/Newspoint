using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newspoint.Application.Services;
using Newspoint.Application.Services.Interfaces;
using Newspoint.Domain.Entities;
using Newspoint.Server.Areas.Public.DTOs;
using Newspoint.Server.Extensions;

namespace Newspoint.Server.Areas.Public.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IMapper _mapper;

    public CommentController(
        ICommentService commentService,
        IMapper mapper)
    {
        _commentService = commentService;
        _mapper = mapper;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var result = await _commentService.GetById(id);
        return this.ToActionResult<Comment, CommentDto>(result, _mapper);
    }
}