    using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;

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
}