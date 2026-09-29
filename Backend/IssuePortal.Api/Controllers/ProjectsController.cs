using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using IssuePortal.Api.Models;
using Microsoft.AspNetCore.Authorization;
using IssuePortal.Api.Extensions;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectsController(ProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetProjects()
    {
        var projects = await _projectService.GetAllProjectsAsync(User.ToCurrentUser());

        return Ok(projects);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject(CreateProjectDto dto)
    {
        var createdProject = await _projectService.CreateProjectAsync(dto, User.ToCurrentUser().Id);

        return CreatedAtAction(
            nameof(GetProjects),
            null,
            createdProject
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProjectById(int id)
    {
        var project = await _projectService.GetProjectByIdAsync(id, User.ToCurrentUser());

        if (project == null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto dto)
    {
        var updatedProject = await _projectService.UpdateProjectAsync(id, dto);

        if (updatedProject == null)
        {
            return NotFound();
        }

        return Ok(updatedProject);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var deleted = await _projectService.DeleteProjectAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id}/members")]
    public async Task<IActionResult> GetMembers(int id)
    {
        var members = await _projectService.GetMembersAsync(id, User.ToCurrentUser());

        if (members == null)
        {
            return NotFound();
        }

        return Ok(members);
    }

    [HttpPost("{id}/members")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddMember(int id, AddProjectMemberDto dto)
    {
        var result = await _projectService.AddMemberAsync(id, dto);

        if (result.Member == null)
        {
            if (result.Error == null)
            {
                return NotFound();
            }

            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetMembers),
            new { id },
            result.Member
        );
    }

    [HttpDelete("{id}/members/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveMember(int id, int userId)
    {
        var removed = await _projectService.RemoveMemberAsync(id, userId);

        if (!removed)
        {
            return NotFound();
        }

        return NoContent();
    }
}
