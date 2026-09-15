using IssuePortal.Api.Models;
using IssuePortal.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace IssuePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IssuesController : ControllerBase
{
    private readonly IssueService _issueService;

    public IssuesController(IssueService issueService)
    {
        _issueService = issueService;
    }

    [HttpGet]
    public async Task<IActionResult> GetIssues()
    {
        var issues = await _issueService.GetAllIssuesAsync();

        return Ok(issues);
    }

    [HttpPost]
    public async Task<IActionResult> CreateIssue(Issue issue)
    {
        var result = await _issueService.CreateIssueAsync(issue);

        if (result.Issue == null)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetIssueById),
            new { id = result.Issue.Id },
            result.Issue
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateIssue(int id, Issue issue)
    {
        var result = await _issueService.UpdateIssueAsync(id, issue);

        if (result.Issue == null)
        {
            if (result.Error == null)
            {
                return NotFound();
            }

            return BadRequest(result.Error);
        }

        return Ok(result.Issue);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteIssue(int id)
    {
        var deleted = await _issueService.DeleteIssueAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetIssueById(int id)
    {
        var issue = await _issueService.GetIssueByIdAsync(id);

        if (issue == null)
        {
            return NotFound();
        }

        return Ok(issue);
    }
}