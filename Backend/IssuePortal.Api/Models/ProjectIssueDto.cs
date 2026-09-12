namespace IssuePortal.Api.Models;

public class ProjectIssueDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = "Open";

    public string Priority { get; set; } = "Medium";
}