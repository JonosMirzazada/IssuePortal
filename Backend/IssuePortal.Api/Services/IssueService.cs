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

    public async Task<Issue?> GetIssueByIdAsync(int id)
    {
        return await _context.Issues.FindAsync(id);
    }

    public async Task<Issue> CreateIssueAsync(Issue issue)
    {
        issue.CreatedAt = DateTime.UtcNow;
        issue.UpdatedAt = DateTime.UtcNow;

        _context.Issues.Add(issue);
        await _context.SaveChangesAsync();

        return issue;
    }

    public async Task<Issue?> UpdateIssueAsync(int id, Issue issue)
    {
        var existingIssue = await _context.Issues.FindAsync(id);

        if (existingIssue == null)
        {
            return null;
        }

        existingIssue.Title = issue.Title;
        existingIssue.Description = issue.Description;
        existingIssue.Status = issue.Status;
        existingIssue.Priority = issue.Priority;
        existingIssue.ProjectId = issue.ProjectId;
        existingIssue.AssignedUserId = issue.AssignedUserId;
        existingIssue.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return existingIssue;
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