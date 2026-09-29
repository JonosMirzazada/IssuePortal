using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class IssueService
{
    private readonly IssuePortalDbContext _context;
    private readonly ProjectAccessService _projectAccess;

    public IssueService(IssuePortalDbContext context, ProjectAccessService projectAccess)
    {
        _context = context;
        _projectAccess = projectAccess;
    }

    public async Task<List<Issue>> GetAllIssuesAsync(IssueQueryDto query, CurrentUser user)
    {
        var memberProjectIds = _projectAccess.MemberProjectIds(user);

        var issues = _context.Issues
            .Where(i => user.IsAdmin || memberProjectIds.Contains(i.ProjectId));

        if (query.ProjectId.HasValue)
        {
            issues = issues.Where(i => i.ProjectId == query.ProjectId.Value);
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            issues = issues.Where(i => i.Status == query.Status);
        }

        if (!string.IsNullOrEmpty(query.Priority))
        {
            issues = issues.Where(i => i.Priority == query.Priority);
        }

        var assignedUserId = query.AssignedToMe ? user.Id : query.AssignedUserId;

        if (assignedUserId.HasValue)
        {
            issues = issues.Where(i => i.AssignedUserId == assignedUserId.Value);
        }

        return await issues
            .OrderByDescending(i => i.UpdatedAt)
            .ToListAsync();
    }

    public async Task<IssueDto?> GetIssueByIdAsync(int id, CurrentUser user)
    {
        var issue = await _context.Issues
            .Include(i => i.Comments)
            .ThenInclude(c => c.User)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (issue == null || !await _projectAccess.CanAccessProjectAsync(issue.ProjectId, user))
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

    public async Task<(Issue? Issue, string? Error)> CreateIssueAsync(CreateIssueDto dto, CurrentUser user)
    {
        // Same message whether the project is missing or inaccessible,
        // so non-members cannot probe which project IDs exist.
        if (!await _projectAccess.CanAccessProjectAsync(dto.ProjectId, user))
        {
            return (null, $"Project with ID {dto.ProjectId} does not exist.");
        }

        if (dto.AssignedUserId.HasValue
            && !await _projectAccess.IsMemberAsync(dto.ProjectId, dto.AssignedUserId.Value))
        {
            return (null, $"User with ID {dto.AssignedUserId.Value} is not a member of project {dto.ProjectId}.");
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

    public async Task<(Issue? Issue, string? Error)> UpdateIssueAsync(int id, UpdateIssueDto dto, CurrentUser user)
    {
        var existingIssue = await _context.Issues.FindAsync(id);

        if (existingIssue == null || !await _projectAccess.CanAccessProjectAsync(existingIssue.ProjectId, user))
        {
            return (null, null);
        }

        // Same message whether the project is missing or inaccessible,
        // so non-members cannot probe which project IDs exist.
        if (!await _projectAccess.CanAccessProjectAsync(dto.ProjectId, user))
        {
            return (null, $"Project with ID {dto.ProjectId} does not exist.");
        }

        if (dto.AssignedUserId.HasValue
            && !await _projectAccess.IsMemberAsync(dto.ProjectId, dto.AssignedUserId.Value))
        {
            return (null, $"User with ID {dto.AssignedUserId.Value} is not a member of project {dto.ProjectId}.");
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