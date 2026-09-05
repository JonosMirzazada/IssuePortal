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
}