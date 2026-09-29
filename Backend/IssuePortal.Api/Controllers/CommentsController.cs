using IssuePortal.Api.Models;
using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IssuePortal.Api.Extensions;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;

    public CommentsController(CommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetComments()
    {
        var comments = await _commentService.GetAllCommentsAsync(User.ToCurrentUser());

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id, User.ToCurrentUser());

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(CreateCommentDto dto)
    {
        var result = await _commentService.CreateCommentAsync(dto, User.ToCurrentUser());

        if (result.Comment == null)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetCommentById),
            new { id = result.Comment.Id },
            result.Comment
        );
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto dto)
    {
        var updatedComment = await _commentService.UpdateCommentAsync(id, dto);

        if (updatedComment == null)
        {
            return NotFound();
        }

        return Ok(updatedComment);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteComment(int id)
    {
        var deleted = await _commentService.DeleteCommentAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}