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

    public async Task<List<CommentDto>> GetAllCommentsAsync()
    {
        return await _context.Comments
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                IssueId = c.IssueId,
                IssueTitle = c.Issue != null ? c.Issue.Title : string.Empty,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.Name : string.Empty
            })
            .ToListAsync();
    }

    public async Task<CommentDto?> GetCommentByIdAsync(int id)
    {
        return await _context.Comments
            .Where(c => c.Id == id)
            .Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                IssueId = c.IssueId,
                IssueTitle = c.Issue != null ? c.Issue.Title : string.Empty,
                UserId = c.UserId,
                UserName = c.User != null ? c.User.Name : string.Empty
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CommentDto> CreateCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.UtcNow;

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        var createdComment = await _context.Comments
            .Include(c => c.Issue)
            .Include(c => c.User)
            .FirstAsync(c => c.Id == comment.Id);

        return ToDto(createdComment);
    }

    public async Task<CommentDto?> UpdateCommentAsync(int id, Comment comment)
    {
        var existingComment = await _context.Comments.FindAsync(id);

        if (existingComment == null)
        {
            return null;
        }

        existingComment.Content = comment.Content;

        await _context.SaveChangesAsync();

        var updatedComment = await _context.Comments
            .Include(c => c.Issue)
            .Include(c => c.User)
            .FirstAsync(c => c.Id == id);

        return ToDto(updatedComment);
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

    private static CommentDto ToDto(Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            IssueId = comment.IssueId,
            IssueTitle = comment.Issue?.Title ?? string.Empty,
            UserId = comment.UserId,
            UserName = comment.User?.Name ?? string.Empty
        };
    }
}