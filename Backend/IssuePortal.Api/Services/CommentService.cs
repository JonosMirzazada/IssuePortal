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
}