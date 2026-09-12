
using IssuePortal.Api.Models;
using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var comments = await _commentService.GetAllCommentsAsync();

        return Ok(comments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCommentById(int id)
    {
        var comment = await _commentService.GetCommentByIdAsync(id);

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(comment);
    }

    [HttpPost]
    public async Task<IActionResult> CreateComment(CommentDto dto)
    {
        var createdComment = await _commentService.CreateCommentAsync(dto);

        return CreatedAtAction(
            nameof(GetCommentById),
            new { id = createdComment.Id },
            createdComment
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment(int id, Comment comment)
    {
        var updatedComment = await _commentService.UpdateCommentAsync(id, comment);

        if (updatedComment == null)
        {
            return NotFound();
        }

        return Ok(updatedComment);
    }

    [HttpDelete("{id}")]
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
