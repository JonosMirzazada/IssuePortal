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
public async Task<ProjectDto?> GetProjectByIdAsync(int id)
{
    var project = await _context.Projects
        .Include(p => p.Issues)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (project == null)
    {
        return null;
    }

    return new ProjectDto
    {
        Id = project.Id,
        Name = project.Name,
        Description = project.Description,
        CreatedAt = project.CreatedAt,

        Issues = project.Issues.Select(i => new ProjectIssueDto
        {
            Id = i.Id,
            Title = i.Title,
            Status = i.Status,
            Priority = i.Priority
        }).ToList()
    };
}

public async Task<Project?> UpdateProjectAsync(int id, Project project)
{
    var existingProject = await _context.Projects.FindAsync(id);

    if (existingProject == null)
    {
        return null;
    }

    existingProject.Name = project.Name;
    existingProject.Description = project.Description;

    await _context.SaveChangesAsync();

    return existingProject;
}

public async Task<bool> DeleteProjectAsync(int id)
{
    var project = await _context.Projects.FindAsync(id);

    if (project == null)
    {
        return false;
    }

    _context.Projects.Remove(project);

    await _context.SaveChangesAsync();

    return true;
}

}