using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class CommentService
{
    private readonly IssuePortalDbContext _context;

    public CommentService(IssuePortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<Comment>> GetAllCommentsAsync()
    {
        return await _context.Comments.ToListAsync();
    }

    public async Task<Comment?> GetCommentByIdAsync(int id)
    {
        return await _context.Comments.FindAsync(id);
    }

    public async Task<Comment> CreateCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment?> UpdateCommentAsync(int id, Comment comment)
{
    var existingComment = await _context.Comments.FindAsync(id);

    if (existingComment == null)
    {
        return null;
    }

    existingComment.Content = comment.Content;

    await _context.SaveChangesAsync();

    return existingComment;
}

public async Task<bool> DeleteCommentAsync(int id)
{
    var comment = await _context.Comments.FindAsync(id);

    if (comment == null)
    {
        return false;
    }

    _context.Comments.Remove(comment);

    await _context.SaveChangesAsync();

    return true;
}
}