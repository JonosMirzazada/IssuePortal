using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class ProjectService
{
    private readonly IssuePortalDbContext _context;

    public ProjectService(IssuePortalDbContext context)
    {
        _context = context;
    }

    public async Task<List<Project>> GetAllProjectsAsync()
    {
        return await _context.Projects.ToListAsync();
    }

    public async Task<Project> CreateProjectAsync(Project project)
{
    project.CreatedAt = DateTime.UtcNow;

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return project;
}
}