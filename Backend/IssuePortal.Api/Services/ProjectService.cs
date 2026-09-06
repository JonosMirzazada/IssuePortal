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
public async Task<Project?> GetProjectByIdAsync(int id)
{
    return await _context.Projects.FindAsync(id);
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