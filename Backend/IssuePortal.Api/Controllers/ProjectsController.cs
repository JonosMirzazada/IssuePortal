using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;
using IssuePortal.Api.Models;


namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var projects = await _projectService.GetAllProjectsAsync();

        return Ok(projects);
    }
    [HttpPost]
public async Task<IActionResult> CreateProject(Project project)
{
    var createdProject = await _projectService.CreateProjectAsync(project);

    return CreatedAtAction(
        nameof(GetProjects),
        null,
        createdProject
    );
}


[HttpGet("{id}")]
public async Task<IActionResult> GetProjectById(int id)
{
    var project = await _projectService.GetProjectByIdAsync(id);

    if (project == null)
    {
        return NotFound();
    }

    return Ok(project);
}
[HttpPut("{id}")]
public async Task<IActionResult> UpdateProject(int id, Project project)
{
    var updatedProject = await _projectService.UpdateProjectAsync(id, project);

    if (updatedProject == null)
    {
        return NotFound();
    }

    return Ok(updatedProject);
}

[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProject(int id)
{
    var deleted = await _projectService.DeleteProjectAsync(id);

    if (!deleted)
    {
        return NotFound();
    }

    return NoContent();
}

}