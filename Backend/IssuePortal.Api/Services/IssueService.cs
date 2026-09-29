using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class IssueService
{
    private readonly IssuePortalDbContext _context;

    public IssueService(IssuePortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<Issue>> GetAllIssuesAsync()
    {
        return await _context.Issues.ToListAsync();
    }

    public async Task<IssueDto?> GetIssueByIdAsync(int id)
    {
        var issue = await _context.Issues
            .Include(i => i.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null)
        {
            return null;
        }

        return new IssueDto
        {
            Id = issue.Id,
            Title = issue.Title,
            Description = issue.Description,
            Status = issue.Status,
            Priority = issue.Priority,
            CreatedAt = issue.CreatedAt,
            UpdatedAt = issue.UpdatedAt,
            ProjectId = issue.ProjectId,
            AssignedUserId = issue.AssignedUserId,

            Comments = issue.Comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                IssueId = c.IssueId,
                IssueTitle = issue.Title,
                UserId = c.UserId,
                UserName = c.User?.Name ?? string.Empty
            }).ToList()
        };
    }

    public async Task<(Issue? Issue, string? Error)> CreateIssueAsync(CreateIssueDto dto)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == dto.ProjectId);

        if (!projectExists)
        {
            return (null, $"Project with ID {dto.ProjectId} does not exist.");
        }

        if (dto.AssignedUserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.AssignedUserId.Value);

            if (!userExists)
            {
                return (null, $"User with ID {dto.AssignedUserId.Value} does not exist.");
            }
        }

        var issue = new Issue
        {
            Title = dto.Title,
            Description = dto.Description,
            Status = dto.Status,
            Priority = dto.Priority,
            ProjectId = dto.ProjectId,
            AssignedUserId = dto.AssignedUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Issues.Add(issue);

        await _context.SaveChangesAsync();

        return (issue, null);
    }

    public async Task<(Issue? Issue, string? Error)> UpdateIssueAsync(int id, UpdateIssueDto dto)
    {
        var existingIssue = await _context.Issues.FindAsync(id);

        if (existingIssue == null)
        {
            return (null, null);
        }

        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == dto.ProjectId);

        if (!projectExists)
        {
            return (null, $"Project with ID {dto.ProjectId} does not exist.");
        }

        if (dto.AssignedUserId.HasValue)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.Id == dto.AssignedUserId.Value);

            if (!userExists)
            {
                return (null, $"User with ID {dto.AssignedUserId.Value} does not exist.");
            }
        }

        existingIssue.Title = dto.Title;
        existingIssue.Description = dto.Description;
        existingIssue.Status = dto.Status;
        existingIssue.Priority = dto.Priority;
        existingIssue.ProjectId = dto.ProjectId;
        existingIssue.AssignedUserId = dto.AssignedUserId;
        existingIssue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return (existingIssue, null);
    }

    public async Task<bool> DeleteIssueAsync(int id)
    {
        var issue = await _context.Issues.FindAsync(id);

        if (issue == null)
        {
            return false;
        }

        _context.Issues.Remove(issue);

        await _context.SaveChangesAsync();

        return true;
    }
}