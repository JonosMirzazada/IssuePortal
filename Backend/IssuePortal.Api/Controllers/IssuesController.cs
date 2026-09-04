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
        var createdIssue = await _issueService.CreateIssueAsync(issue);

        return CreatedAtAction(
            nameof(GetIssueById),
            new { id = createdIssue.Id },
            createdIssue
        );
    }

    [HttpPut("{id}")]
public async Task<IActionResult> UpdateIssue(int id, Issue issue)
{
    var updatedIssue = await _issueService.UpdateIssueAsync(id, issue);

    if (updatedIssue == null)
    {
        return NotFound();
    }

    return Ok(updatedIssue);
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