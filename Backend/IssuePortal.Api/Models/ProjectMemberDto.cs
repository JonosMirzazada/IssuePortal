namespace IssuePortal.Api.Models;

public class ProjectMemberDto
{
    public int UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime JoinedAt { get; set; }
}
