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
}