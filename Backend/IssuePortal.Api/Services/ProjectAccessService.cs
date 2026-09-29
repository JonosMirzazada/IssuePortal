using IssuePortal.Api.Data;
using IssuePortal.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace IssuePortal.Api.Services;

// Admins can access every project. Other users can only access
// projects they are a member of.
public class ProjectAccessService
{
    private readonly IssuePortalDbContext _context;

    public ProjectAccessService(IssuePortalDbContext context)
    {
        _context = context;
    }

    // IDs of the projects the user is a member of. Meant to be composed
    // into other queries, e.g. .Where(i => user.IsAdmin || ids.Contains(i.ProjectId)).
    public IQueryable<int> MemberProjectIds(CurrentUser user)
    {
        return _context.ProjectMembers
            .Where(pm => pm.UserId == user.Id)
            .Select(pm => pm.ProjectId);
    }

    // True if the project exists and the user may access it.
    public async Task<bool> CanAccessProjectAsync(int projectId, CurrentUser user)
    {
        if (user.IsAdmin)
        {
            return await _context.Projects.AnyAsync(p => p.Id == projectId);
        }

        return await IsMemberAsync(projectId, user.Id);
    }

    public async Task<bool> IsMemberAsync(int projectId, int userId)
    {
        return await _context.ProjectMembers
            .AnyAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
    }
}
