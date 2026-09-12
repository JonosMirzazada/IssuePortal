namespace IssuePortal.Api.Models;

public class ProjectDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<ProjectIssueDto> Issues { get; set; } = new();
}