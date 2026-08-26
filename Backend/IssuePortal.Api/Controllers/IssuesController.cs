using IssuePortal.Api.Models;
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
            new Issue
            {
                Id = 1,
                Title = "Login fungerar inte",
                Description = "Användaren kan inte logga in.",
                Status = "Open",
                Priority = "High"
            },
            new Issue
            {
                Id = 2,
                Title = "Dashboard behöver fixas",
                Description = "Dashboard laddar inte korrekt.",
                Status = "In Progress",
                Priority = "Medium"
            }
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetIssueById(int id)
    {
        if (id == 1)
        {
            return Ok(new Issue
            {
                Id = 1,
                Title = "Login fungerar inte",
                Description = "Användaren kan inte logga in.",
                Status = "Open",
                Priority = "High"
            });
        }

        if (id == 2)
        {
            return Ok(new Issue
            {
                Id = 2,
                Title = "Dashboard behöver fixas",
                Description = "Dashboard laddar inte korrekt.",
                Status = "In Progress",
                Priority = "Medium"
            });
        }

        return NotFound();
    }

    [HttpPost]
    public IActionResult CreateIssue(Issue issue)
    {
        issue.Id = 3;

        return CreatedAtAction(
            nameof(GetIssueById),
            new { id = issue.Id },
            issue
        );
    }
}