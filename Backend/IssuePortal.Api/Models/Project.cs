namespace IssuePortal.Api.Models;

public class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public List<Issue> Issues { get; set; } = new();

    public List<ProjectMember> Members { get; set; } = new();
}
