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
}