
using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class CommentService
{
    private readonly IssuePortalDbContext _context;
    private readonly ProjectAccessService _projectAccess;

    public CommentService(IssuePortalDbContext context, ProjectAccessService projectAccess)
    {
        _context = context;
        _projectAccess = projectAccess;
    }

    public async Task<List<CommentDto>> GetAllCommentsAsync(CurrentUser user)
    {
        var memberProjectIds = _projectAccess.MemberProjectIds(user);

        return await _context.Comments
            .Where(c => user.IsAdmin || memberProjectIds.Contains(c.Issue!.ProjectId))
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

    public async Task<CommentDto?> GetCommentByIdAsync(int id, CurrentUser user)
    {
        var memberProjectIds = _projectAccess.MemberProjectIds(user);

        return await _context.Comments
            .Where(c => c.Id == id)
            .Where(c => user.IsAdmin || memberProjectIds.Contains(c.Issue!.ProjectId))
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

    public async Task<(CommentDto? Comment, string? Error)> CreateCommentAsync(CreateCommentDto dto, CurrentUser user)
    {
        var issue = await _context.Issues.FindAsync(dto.IssueId);

        // Same message whether the issue is missing or inaccessible
        if (issue == null || !await _projectAccess.CanAccessProjectAsync(issue.ProjectId, user))
        {
            return (null, $"Issue with ID {dto.IssueId} does not exist.");
        }

        var comment = new Comment
        {
            Content = dto.Content,
            IssueId = dto.IssueId,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        await _context.SaveChangesAsync();

        var createdComment = await _context.Comments
            .Include(c => c.Issue)
            .Include(c => c.User)
            .FirstAsync(c => c.Id == comment.Id);

        return (ToDto(createdComment), null);
    }

    public async Task<CommentDto?> UpdateCommentAsync(int id, UpdateCommentDto dto)
    {
        var existingComment = await _context.Comments.FindAsync(id);

        if (existingComment == null)
        {
            return null;
        }

        existingComment.Content = dto.Content;

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