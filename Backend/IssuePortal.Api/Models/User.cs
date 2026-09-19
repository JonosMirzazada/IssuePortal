namespace IssuePortal.Api.Models;

public class User
{
    public int Id { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public List<Comment> Comments { get; set; } = new();

    public List<Issue> AssignedIssues { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}