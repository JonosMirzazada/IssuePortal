using Microsoft.AspNetCore.Mvc;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    [HttpGet]
    public IActionResult GetIssues()
    {
        return Ok(new[]
        {
            new
            {
                Id = 1,
                Title = "Login fungerar inte",
                Status = "Open",
                Priority = "High"
            },
            new
            {
                Id = 2,
                Title = "Dashboard behöver fixas",
                Status = "In Progress",
                Priority = "Medium"
            }
        });
    }
}