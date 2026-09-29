using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

public class ProjectService
{
    private readonly IssuePortalDbContext _context;
    private readonly ProjectAccessService _projectAccess;

    public ProjectService(IssuePortalDbContext context, ProjectAccessService projectAccess)
    {
        _context = context;
        _projectAccess = projectAccess;
    }

    public async Task<List<Project>> GetAllProjectsAsync(CurrentUser user)
    {
        var memberProjectIds = _projectAccess.MemberProjectIds(user);

        return await _context.Projects
            .Where(p => user.IsAdmin || memberProjectIds.Contains(p.Id))
            .ToListAsync();
    }

    public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, int creatorUserId)
{
    var project = new Project
    {
        Name = dto.Name,
        Description = dto.Description,
        CreatedAt = DateTime.UtcNow
    };

    // The creator automatically becomes a member of the project
    project.Members.Add(new ProjectMember
    {
        UserId = creatorUserId,
        JoinedAt = DateTime.UtcNow
    });

    _context.Projects.Add(project);
    await _context.SaveChangesAsync();

    return ToDto(project);
}
public async Task<ProjectDto?> GetProjectByIdAsync(int id, CurrentUser user)
{
    if (!await _projectAccess.CanAccessProjectAsync(id, user))
    {
        return null;
    }

    var project = await _context.Projects
        .Include(p => p.Issues)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (project == null)
    {
        return null;
    }

    return ToDto(project);
}

public async Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto dto)
{
    var existingProject = await _context.Projects.FindAsync(id);

    if (existingProject == null)
    {
        return null;
    }

    existingProject.Name = dto.Name;
    existingProject.Description = dto.Description;

    await _context.SaveChangesAsync();

    return ToDto(existingProject);
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

    public async Task<List<ProjectMemberDto>?> GetMembersAsync(int projectId, CurrentUser user)
    {
        if (!await _projectAccess.CanAccessProjectAsync(projectId, user))
        {
            return null;
        }

        return await _context.ProjectMembers
            .Where(pm => pm.ProjectId == projectId)
            .OrderBy(pm => pm.JoinedAt)
            .Select(pm => new ProjectMemberDto
            {
                UserId = pm.UserId,
                UserName = pm.User != null ? pm.User.Name : string.Empty,
                Email = pm.User != null ? pm.User.Email : string.Empty,
                JoinedAt = pm.JoinedAt
            })
            .ToListAsync();
    }

    public async Task<(ProjectMemberDto? Member, string? Error)> AddMemberAsync(int projectId, AddProjectMemberDto dto)
    {
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == projectId);

        if (!projectExists)
        {
            return (null, null);
        }

        var user = await _context.Users.FindAsync(dto.UserId);

        if (user == null)
        {
            return (null, $"User with ID {dto.UserId} does not exist.");
        }

        var alreadyMember = await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == dto.UserId);

        if (alreadyMember)
        {
            return (null, $"User with ID {dto.UserId} is already a member of this project.");
        }

        var member = new ProjectMember
        {
            ProjectId = projectId,
            UserId = dto.UserId,
            JoinedAt = DateTime.UtcNow
        };

        _context.ProjectMembers.Add(member);

        await _context.SaveChangesAsync();

        return (new ProjectMemberDto
        {
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email,
            JoinedAt = member.JoinedAt
        }, null);
    }

    public async Task<bool> RemoveMemberAsync(int projectId, int userId)
    {
        var member = await _context.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        if (member == null)
        {
            return false;
        }

        _context.ProjectMembers.Remove(member);

        await _context.SaveChangesAsync();

        return true;
    }

    private static ProjectDto ToDto(Project project)
    {
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
}
